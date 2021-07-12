using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmHouseRedone.UI
{
    public class Animation
    {
        public float animationTracker;

        public List<KeyFrame> frames;

        public Vector2 animationController;

        public bool isFinished;

        public Animation(List<KeyFrame> frames)
        {
            this.frames = frames;
            isFinished = false;
            animationController = Vector2.Zero;
            animationTracker = 0f;
        }

        public void Update(GameTime time)
        {
            if (isFinished)
            {
                animationTracker = 0f;
                return;
            }
            animationTracker += (float)time.ElapsedGameTime.TotalSeconds;
            //Logger.Log("Animation tracker: " + animationTracker);
            for(int i = 0; i < frames.Count - 1; i++)
            {
                float timeA = frames[i].time;
                float timeB = frames[i + 1].time;
                if (animationTracker < timeA)
                    animationController = frames[i].destination;
                if (animationTracker >= timeA && animationTracker < timeB)
                {
                    float lerpTime = animationTracker - timeA;
                    float animationTime = timeB - timeA;
                    //Logger.Log($"Animating. lerpTime: {lerpTime} animationTime: {animationTime}");
                    if (frames[i].animationStyle == 0)
                        animationController = new Vector2(
                            Utility.Lerp(frames[i].destination.X, frames[i + 1].destination.X, (float)(lerpTime / animationTime)),
                            Utility.Lerp(frames[i].destination.Y, frames[i + 1].destination.Y, (float)(lerpTime / animationTime))
                        );
                    else
                    {
                        animationController = new Vector2(
                            CubicInterpolate(frames[i].destination.X, frames[i + 1].destination.X, 0, 0, 0, 0, 0, 0, (float)(lerpTime / animationTime)),
                            CubicInterpolate(frames[i].destination.Y, frames[i + 1].destination.Y, -0.83f, -8.3f, 0.05f, -1f, -0.45f, 1f, (float)(lerpTime / animationTime))
                        );
                    }
                }
            }
            if (animationTracker >= frames[frames.Count - 1].time)
                isFinished = true;
        }

        public float CubicInterpolate(float initialValue, float destinationValue, float i, float j, float a, float b, float c, float d, float factor)
        {
            return a*Cubic(0, i, 0, j, factor) + b*Cubic(1, i, 0, j, factor) + c*Cubic(0, i, 1, j, factor) + d*Cubic(1, i, 1, j, factor) * (destinationValue - initialValue) + initialValue;
        }

        public float Cubic(float a, float b, float c, float d, float factor)
        {
            float pZero = (float)(2 * Math.Pow(factor, 3) - 3 * Math.Pow(factor, 2) + 1);
            float mZero = (float)(Math.Pow(factor, 3) - 2 * Math.Pow(factor, 2) + factor);
            float pOne = (float)(-2 * Math.Pow(factor, 3) + 3 * Math.Pow(factor, 2));
            float mOne = (float)(Math.Pow(factor, 3) - Math.Pow(factor, 2));

            return a*pZero + b*mZero + c*pOne + d*mOne;
        }
    }
}
