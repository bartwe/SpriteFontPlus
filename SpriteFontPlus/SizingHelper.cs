namespace SpriteFontPlus {
    internal class SizingHelper {
        private static readonly int[] Primes = { 3, 5, 7, 11, 17, 23, 37, 53, 79, 113, 163, 229, 331, 463, 653, 919, 1289, 1811, 2539, 3557, 4987, 6983, 9781, 13693, 19181, 26861, 37607, 52667, 73751, 103289, 144611, 202471, 283463, 396871, 555637, 777901, 1089091, 1524763, 2134697, 2988607, 4184087, 5857727, 8200847, 11481199, 16073693, 22503181, 31504453, 44106241, 61748749, 86448259, 121027583, 169438627, 237214097, 332099741, 464939639, 650915521, 911281733, 1275794449, 1786112231 };

        public static int GetSizingPrime(int min) {
            for (var index = 0; index < Primes.Length; ++index) {
                var num = Primes[index];
                if (num >= min) {
                    return num;
                }
            }
            throw new("Trying to find a too large prime.");
        }

        public static int NextSizingPrime(int min) {
            for (var index = 0; index < Primes.Length; ++index) {
                var num = Primes[index];
                if (num > min) {
                    return num;
                }
            }
            throw new("Trying to find a too large prime.");
        }
    }
}