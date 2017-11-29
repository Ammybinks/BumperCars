using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using SpriteLibrary;

namespace BumperCars
{
    // the Car will inherit from the standard Sprite
    public class Car : SpriteLibrary.Sprite
    {
        // this variable represents the player score
        public int Score = 0;

        // turn to the left or right as specified by the 
        // positive or negative angle change (in degrees)
        public void Steer(double angleChange)
        {
            // cars will always travel in the same
            // direction as they are pointing
            ChangeDirectionAngle(angleChange);
            ChangeRotationAngle(angleChange);
        }

        // this method will move the sprite and reflect from the screen edges
        public bool MoveAndReflect(float screenSizeX, float screenSizeY)
        {
            // assume no collision happened
            bool retValue = false;

            // move the sprite normally
            if (!Move())
                return retValue;

            // get the bounding rectangle for the car
            Rectangle boundingRect = GetBoundingRectangle();

            // get the current car velocity
            Vector2 velocity = GetVelocity();

            // we are going to reduce the speed by a factor of 5 on collision!
            const int SPEED_REDUCTION_FACTOR = 5;

            if ((boundingRect.X < 0) && (velocity.X < 0))
            {
                // bounce against the left wall, setting X component negative and
                // dividing both X and Y components by speed reduction factor
                SetVelocity(-velocity.X / SPEED_REDUCTION_FACTOR, velocity.Y / SPEED_REDUCTION_FACTOR);
                UpperLeft.X = 0; // ensure we are on-screen
                retValue = true;    // yes there was a collision
            }

            if (((boundingRect.X + boundingRect.Width) > (screenSizeX)) && (velocity.X > 0))
            {
                // bounce against the right wall, setting X component negative and
                // dividing both X and Y components by speed reduction factor
                SetVelocity(-velocity.X / SPEED_REDUCTION_FACTOR, velocity.Y / SPEED_REDUCTION_FACTOR);
                UpperLeft.X = screenSizeX - boundingRect.Width - 1; // ensure we are on-screen
                retValue = true;    // yes there was a collision
            }

            if ((boundingRect.Y < 0) && (velocity.Y < 0))
            {
                // bounce against the top wall, setting Y component negative and
                // dividing both X and Y components by speed reduction factor
                SetVelocity(velocity.X / SPEED_REDUCTION_FACTOR, -velocity.Y / SPEED_REDUCTION_FACTOR);
                UpperLeft.Y = 0; // ensure we are on-screen
                retValue = true;    // yes there was a collision
            }

            if (((boundingRect.Y + boundingRect.Height) > screenSizeY) && (velocity.Y > 0))
            {
                // bounce against the bottom wall, setting Y component negative and
                // dividing both X and Y components by speed reduction factor
                SetVelocity(velocity.X / SPEED_REDUCTION_FACTOR, -velocity.Y / SPEED_REDUCTION_FACTOR);
                UpperLeft.Y = screenSizeY - boundingRect.Height - 1; // ensure we are on-screen
                retValue = true;    // yes there was a collision
            }

            // make sure rotation angle has changed too, becuase cars always face direction of travel
            RotationAngle = GetDirectionAngle();

            return retValue;
        }

        // this method will determine if the two bumper cars have hit each other
        // and, if they have, bounce them from one another as if they were billiard balls
        public bool CheckBump(Car car2, int screenWidth, int screenHeight)
        {
            Car car1 = this;

            // NOTE:  This method approximates the cars as circular objects that 
            // will bounce away from each other at angles corresponding to 
            // circular collision!  Cars are of course not circles, but the approximation
            // works well for this game.
            
            // first get X and Y coordinates for the center of each car
            double radius = car1.GetWidth() / 2;

            double x1 = car1.GetCenter().X + car1.UpperLeft.X;
            double y1 = car1.GetCenter().Y + car1.UpperLeft.Y;
            double x2 = car2.GetCenter().X + car2.UpperLeft.X;
            double y2 = car2.GetCenter().Y + car2.UpperLeft.Y;

            // get the difference in the center coordinates
            double dx = x2 - x1;
            double dy = y2 - y1;

            // find the distance between the car centers
            double distance = Math.Sqrt(dx * dx + dy * dy);

            // if the car "circles" are not touching 
            if (distance > (2 * radius))
                return false; // no bounce
            
            // get velocity components for boht cars
            double vx1 = car1.GetVelocity().X;
            double vy1 = car1.GetVelocity().Y;
            double vx2 = car2.GetVelocity().X;
            double vy2 = car2.GetVelocity().Y;

            // assume perfect elasticity and equal mass
            double ed = 1.0;
            double mass1 = 1.0;
            double mass2 = 1.0;

            // Since we are only checking for collisions at discrete times
            // it's possible that the two car "circles" are now overlapped
            // instead of perfectly touching.  Need to correct for that and
            // move them backwards slightly so they are perfectly touching.
            // Otherwise the cards will appear to "stick" together as they
            // repeatedly collide with overlapped circles.
            double vp1 = vx1 * dx / distance + vy1 * dy / distance;
            double vp2 = vx2 * dx / distance + vy2 * dy / distance;

            double dt = (radius + radius - distance) / (vp1 - vp2);

            x1 -= vx1 * dt;
            y2 -= vy1 * dt;
            x2 -= vx2 * dt;
            y2 -= vy2 * dt;

            // now we have true distance between them after correction above
            dx = x2 - x1;
            dy = y2 - y1;

            distance = Math.Sqrt(dx * dx + dy * dy);

            // Unit vector in the direction of the collision
            double ax=dx/distance, ay=dy/distance;
            
            // Projection of the velocities in these axes
            double va1=(vx1*ax+vy1*ay), vb1=(-vx1*ay+vy1*ax);
            double va2=(vx2*ax+vy2*ay), vb2=(-vx2*ay+vy2*ax);
            
            // New velocities in these axes (after collision): ed<=1,  for elastic collision ed=1
            double vaP1=va1 + (1+ed)*(va2-va1)/(1+mass1/mass2);
            double vaP2=va2 + (1+ed)*(va1-va2)/(1+mass2/mass1);
            
            // Undo the projections
            vx1=vaP1*ax-vb1*ay;  vy1=vaP1*ay+vb1*ax;// new vx,vy for ball 1 after collision
            vx2=vaP2*ax-vb2*ay;  vy2=vaP2*ay+vb2*ax;// new vx,vy for ball 2 after collision

            // move the cars slightly in the new direction
            x1 += vx1*dt;
            y1 += vy1*dt;
            x2 += vx2*dt;
            y2 += vy2*dt;

            // reposition the cars in terms of UpperLeft coordinates
            car1.UpperLeft.X = (float)x1 - car1.GetCenter().X;
            car1.UpperLeft.Y = (float)y1 - car1.GetCenter().Y;
            car2.UpperLeft.X = (float)x2 - car2.GetCenter().X;
            car2.UpperLeft.Y = (float)y2 - car2.GetCenter().Y;

            // make sure we didn't bounce somebody completely off the screen
            keepOnScreen(car1, screenWidth, screenHeight);
            keepOnScreen(car2, screenWidth, screenHeight);

            // set new direction and speed for each car
            car1.SetVelocity(vx1, vy1);
            car2.SetVelocity(vx2, vy2);

            // keep rotation angles in sync with direction angles
            car1.RotationAngle = car1.GetDirectionAngle();
            car2.RotationAngle = car2.GetDirectionAngle();

            return true;    // yes there was a collision

        }

        // makes sure that a car's position is entirely on the screen
        // after a bounce
        private void keepOnScreen(Car car, int screenWidth, int screenHeight)
        {
            if (car.UpperLeft.X < 0)
                car.UpperLeft.X = 0;
            if (car.UpperLeft.Y < 0)
                car.UpperLeft.Y = 0;
            if (car.UpperLeft.X + car.GetWidth() > screenWidth)
                car.UpperLeft.X = screenWidth = car.GetWidth();
            if (car.UpperLeft.Y + car.GetHeight() > screenHeight)
                car.UpperLeft.Y = screenHeight + car.GetHeight();
        }
    }
}
