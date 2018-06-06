using System;

namespace MetaphoneBr {
    public static class PhoneticExtensions {
        public static string ToPhonetic (
            this string str,
            IPhonetic phonetic) {
            return phonetic.Translate (str);
        }
    }
}