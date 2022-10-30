using System;
using System.Text;

namespace QUTyTest.Models.Sequencing
{
    public class SequenceChecksum
    {
        private const string Base64 = "ABCDEFGHIJKMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        public static string AddChecksum(string sequence, char initial = 'u')
        {
            var bytes = Encoding.ASCII.GetBytes(initial + sequence);
            var total = 0;
            foreach (var byt in bytes)
            {
                total += byt;
            }

            total = total & 0x3F;
            
            return sequence + Base64[total];
        }


        public static bool ConfirmChecksum(string sequence, char initial)
        {

            var bytes = Encoding.ASCII.GetBytes(initial +  sequence.Substring(0, 32));
            var checksum = Base64.IndexOf(sequence[32]);

            var sum = 0;
            foreach(var byt in bytes)
            {
                sum += byt;
            }

            sum = sum & 0x3F;
            return sum == checksum;
        }
    }
}