using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace Object_Delivery_Service_Tool
{
    public static class ObjectDeliveryService
    {
        private static readonly string ObjectDeliveryServiceDomain = "http://secure2.nintendo.co.jp/ngc/gaej_objcenter.cgi";

        private static readonly int[] DecorationPrices = new int[15]
        {
            18_000, 14_000, 27_000, 28_000, 15_000,
            18_000, 17_000, 26_000, 32_000, 19_000,
            20_000, 24_000, 13_000, 14_000, 30_000
        };

        private static readonly string[] AFe_CharList = new string[256]
        {
            "あ", "い", "う", "え", "お", "か", "き", "く", "け", "こ", "さ", "し", "す", "せ", "そ", "た",
            "ち", "つ", "て", "と", "な", "に", "ぬ", "ね", "の", "は", "ひ", "ふ", "へ", "ほ", "ま", "み",
            " ", "!", "\"", "む", "め", "%", "&", "'", "(", ")", "~", "♥", ", ", "-", ".", "♪",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", "🌢", "<", "+", ">", "?",
            "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "も", "💢", "や", "ゆ", "_",
            "よ", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
            "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "ら", "り", "る", "れ", "�",
            "□", "。", "｢", "｣", "、", "･", "ヲ", "ァ", "ィ", "ゥ", "ェ", "ォ", "ャ", "ュ", "ョ", "ッ",
            "ー", "ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ", "サ", "シ", "ス", "セ", "ソ",
            "タ", "チ", "ツ", "テ", "ト", "ナ", "ニ", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ", "マ",
            "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ", "ル", "レ", "ロ", "ワ", "ン", "ヴ", "☺",
            "ろ", "わ", "を", "ん", "ぁ", "ぃ", "ぅ", "ぇ", "ぉ", "ゃ", "ゅ", "ょ", "っ", "\n", "ガ", "ギ",
            "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ", "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ", "ブ",
            "ベ", "ボ", "パ", "ピ", "プ", "ペ", "ポ", "が", "ぎ", "ぐ", "げ", "ご", "ざ", "じ", "ず", "ぜ",
            "ぞ", "だ", "ぢ", "づ", "で", "ど", "ば", "び", "ぶ", "べ", "ぼ", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ"
        };

        private static bool IsURLAlive(string url)
        {
            try
            {
                var request = WebRequest.Create(url);
                request.Timeout = 1000;
                return ((HttpWebResponse)request.GetResponse())?.StatusCode == HttpStatusCode.OK;
            }
            catch // If we've hit an exception it's most likely due to a timeout.
            {
                return false;
            }
        }

        private static byte[] String2ACBytes(string Input, int MaxLength = 6)
        {
            byte[] Output = new byte[MaxLength];
            for (int i = 0; i < MaxLength; i++)
            {
                int CharacterValue = i >= Input.Length ? 0x20 : Array.IndexOf(AFe_CharList, Input.Substring(i, 1));
                if (CharacterValue < 0 || CharacterValue > 255)
                {
                    Console.WriteLine(string.Format("Character [{0}] was not present in the database!", Input.Substring(i, 1)));
                    CharacterValue = 0x20; // Set to space for now
                }

                Output[i] = (byte)CharacterValue;
            }

            return Output;
        }

        private static string PadString(string input)
        {
            while (input.Length < 6)
            {
                input += " ";
            }

            return input;
        }

        public static string EncodeString(string Input)
        {
            if (Input.Length < 1)
            {
                return "";
            }

            byte[] StringData = String2ACBytes(Input);
            string Output = StringData[0].ToString();

            for (int i = 1; i < StringData.Length; i++)
            {
                Output += "c" + StringData[i];
            }

            return Output;
        }

        private static string DecodeEncodedPOSTResult(string Result)
        {
            if (Result.Contains("cgi_status=OK"))
            {
                if (Result.Contains("aikotoba_code="))
                {
                    Console.WriteLine("POST Result: " + Result);
                    string EncodedPassword = Result.Substring(Result.IndexOf("aikotoba_code=") + 14);
                    string[] Characters = EncodedPassword.Split('c');
                    string Password = "";
                    for (int i = 0; i < Characters.Length; i++)
                    {
                        if (byte.TryParse(Characters[i], out byte Index))
                        {
                            if (i > 0 && i % 16 == 0)
                            {
                                Password += "\r\n";
                            }

                            Password += AFe_CharList[Index];
                        }
                        else
                        {
                            Console.WriteLine("Unable to parse index for: " + Characters[i]);
                        }
                    }

                    return Password;
                }
            }

            Console.WriteLine("Failed to get a response from Nintendo's server. Result:\r\n" + Result);
            return "";
        }

        /// <summary>
        /// Retrieves an Object Delivery Service code from Nintendo's servers.
        /// </summary>
        /// <param name="DiscountPercentage">A float value between 0.0f and 0.3f</param>
        /// <param name="ObjectType">An int between 0 and 14</param>
        /// <param name="X_Acre">The X Acre, between 1 and 5</param>
        /// <param name="Y_Acre">The Y Acre, between 2 and 6</param>
        /// <param name="EncodedPlayerName">The encoded player's name</param>
        /// <param name="EncodedTownName">The encoded town name</param>
        /// <returns>string Password</returns>
        private static string GetPasswordFromDomain(float DiscountPercentage, int ObjectType, int X_Acre, int Y_Acre,
            string EncodedPlayerName, string EncodedTownName)
        {
            using (var Client = new WebClient())
            {
                var Values = new NameValueCollection
                {
                    ["discount"] = DiscountPercentage.ToString(),
                    ["objtype"] = ObjectType.ToString(),
                    ["banti"] = X_Acre.ToString(),
                    ["tyome"] = Y_Acre.ToString(),
                    ["player"] = EncodedPlayerName,
                    ["village"] = EncodedTownName
                };

                byte[] ReturnedData = Client.UploadValues(ObjectDeliveryServiceDomain, Values);
                return DecodeEncodedPOSTResult(Encoding.ASCII.GetString(ReturnedData));
            }
        }

        public static string GetPasswordString(float DiscountPercentage, int ObjectType, int X_Acre, int Y_Acre,
            string PlayerName, string TownName)
        {
            // Check if the URL is still alive. If it is, let's use it.
            if (IsURLAlive(ObjectDeliveryServiceDomain))
            {
                return GetPasswordFromDomain(DiscountPercentage, ObjectType, X_Acre, Y_Acre,
                    EncodeString(PlayerName), EncodeString(TownName));
            }
            else // If it isn't, fall back to the Password Library.
            {
                var priceString = ((int)Math.Round(DecorationPrices[ObjectType] * (1.0f - DiscountPercentage), 0)).ToString();

                return PasswordLibrary.Encoder.Encoder.Encode(PasswordLibrary.CodeType.Monument, 0, PadString(TownName),
                    PadString(PlayerName), PadString(priceString), (ushort)ObjectType, ((Y_Acre & 7) << 3) | (X_Acre & 7));
            }
        }

        public static int GetPriceWithDiscount(float Discount, int ObjectType)
        {
            return (int)Math.Round((DecorationPrices[ObjectType] * (1 - Discount)));
        }
    }
}
