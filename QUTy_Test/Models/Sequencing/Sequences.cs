namespace QUTyTest.Models.Sequencing
{
    public static class Sequences
    {
        public static string Build1HzSiren(bool addChecksum = true, char checksumInitial = 'u')
        {
            var builder = new SequenceBuilder();

            builder.AddStep(75, 255, 4, 5);   // Full bright high pitch, 1 sec
            builder.AddStep(75, 20, 1, 1);    // Dimn low pitch, 1 sec
            builder.AddStep(75, 255, 4, 5);   // Full bright high pitch, 1 sec
            builder.AddStep(75, 20, 1, 1);    // Dimn low pitch, 1 sec
            builder.AddStep(75, 255, 4, 5);   // Full bright high pitch, 1 sec
            builder.AddStep(75, 20, 1, 1);    // Dimn low pitch, 1 sec
            builder.AddStep(75, 255, 4, 5);   // Full bright high pitch, 1 sec
            builder.AddStep(0, 0, 0, 0);      // Terminator

            var sequence = builder.Build();

            if (addChecksum)
            {
                sequence = SequenceChecksum.AddChecksum(sequence, checksumInitial);
            }

            return sequence;
        }
    }
}