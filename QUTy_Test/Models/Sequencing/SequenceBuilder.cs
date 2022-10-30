using System;
using System.Collections.Generic;
using System.Linq;

namespace QUTyTest.Models.Sequencing
{
    public class SequenceBuilder
    {
        public List<SequenceStep> Steps { get; } = new List<SequenceStep>();

        public bool AutoAddTerminator { get; set; } = true;

        public void AddStep(SequenceStep step)
        {
            Steps.Add(step);
        }

        public void AddStep(byte duration, byte brightness, byte note, byte octave)
        {
            Steps.Add(new SequenceStep()
            {
                Duration = duration,
                Brightness = brightness,
                Note = note,
                Octave = octave
            });
        }

        public string Build()
        {
            if (AutoAddTerminator)
            {
                var last = Steps.LastOrDefault();
                if (last?.Duration > 0)
                {
                    AddStep(0, 255, 0, 0);
                }
            }

            var buffer = new byte[Steps.Count * 3];
            for (int i = 0; i < Steps.Count; i++)
            {
                var step = Steps[i];
                buffer[i * 3] = step.Duration;
                buffer[(i * 3) + 1] = step.Brightness;
                var octave = (step.Octave << 4) & 0xFF;
                var note = step.Note & 0xF;

                var combined = octave | note;

                buffer[(i * 3) + 2] = (byte)combined;
            }
            return Convert.ToBase64String(buffer);
        }
    }
}