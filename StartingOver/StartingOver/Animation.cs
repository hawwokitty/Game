using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingOver
{
    internal class Animation
    {
        private AnimationManager _animation;

        public Animation(AnimationManager animation)
        {
            _animation = animation;
        }

        public void Play(AnimationManager animation)
        {
            if (_animation == animation) 
                return;

            _animation = animation;
            _animation.activeFrame = 0;
            _animation.counter = 0;
        }

    }
}
