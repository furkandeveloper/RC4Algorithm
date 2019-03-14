using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC4Algorithm.Models;

namespace RC4Algorithm.Controllers
{
    public class HomeController : Controller
    {
        //EncryptionKey

        private string unicodeKey = "";
        private string asciiKey = "";

        private string m_sInClearText = "";
        private string m_sCryptedText = "";

        static public long BoxLength = 255;

        protected byte[] box = new byte[BoxLength];

        static private string GuidKey ="";



        public IActionResult Index()
        {
            RC4ViewModel model = new RC4ViewModel()
            {
                Key = Guid.NewGuid()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(RC4ViewModel model)
        {
            if (ModelState.IsValid)
            {
                GuidKey = model.Key.ToString();
                Anahtar = GuidKey;
                InClear = model.PlainText;
                Sifrele();
                model.ChiperText = m_sCryptedText;
                string a = m_sCryptedText;
                Console.WriteLine(a);
                return View(model);
            }
            ModelState.AddModelError("", "Hata");
            return View(model);
        }

        public bool Sifrele()
        {
            // şifreleme hatalı ise false başarılı ise true dönderecektir.
            bool result = true;
            try
            {
                long i = 0;
                long j = 0;

                Encoding enc_default = Encoding.Default;
                byte[] input = enc_default.GetBytes(this.m_sInClearText);

                byte[] output = new byte[input.Length];

                byte[] lockBox = new byte[BoxLength];
                this.box.CopyTo(lockBox, 0);

                long ChipherLen = input.Length + 1;

                for (long offset = 0; offset < input.Length; offset++)
                {
                    i = (i + 1) % BoxLength;
                    j = (j + lockBox[i]) % BoxLength;
                    byte temp = lockBox[i];
                    lockBox[i] = lockBox[j];
                    lockBox[j] = temp;
                    byte ilk = input[offset];
                    byte son = lockBox[(lockBox[i] + lockBox[j]) % BoxLength];
                    output[offset] = (byte)((int)ilk ^ (int)son); // xor	
                }

                char[] outarrchar = new char[enc_default.GetCharCount(output, 0, output.Length)];
                enc_default.GetChars(output, 0, output.Length, outarrchar, 0);
                this.m_sCryptedText = new string(outarrchar);
            }
            catch (Exception)
            {

                result = false;
            }
            return result;
        }

        public string Anahtar
        {
            // burada box dediğimiz bizim çözüm kümemiz oluşturduğumuz key'in uzunluğuna göre permütasyon işlemi yapacak
            get
            {
                return this.unicodeKey;
            }
            set
            {
                // null'ise anahtarı al
                if (this.unicodeKey != null)
                {
                    this.unicodeKey = GuidKey;
                    long index2 = 0;

                    Encoding ascii = Encoding.ASCII;
                    Encoding unicode = Encoding.Unicode;

                    // girilen karakterler unicode'dur bunların hepsini ascii karakterlere dönüştürür.
                    byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicode.GetBytes(this.unicodeKey));

                    // dönüşen ascii dizisini ascii char olarak kaydededird
                    char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
                    ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
                    this.asciiKey = new string(asciiChars);

                    // girilen metin kadar mod işlemi alınarak karıştırma işlemi yapılacak
                    long KeyLen = unicodeKey.Length;

                    for (long count = 0; count < BoxLength; count++)
                    {
                        this.box[count] = (byte)count;
                    }

                    for (long count = 0; count < BoxLength; count++)
                    {
                        index2 = (index2 + box[count] + asciiChars[count % KeyLen]) % BoxLength;
                        byte temp = box[count];
                        box[count] = box[index2];
                        box[index2] = temp;
                    }
                }
            }
        }

        // sifresiz metin için get ve set metodları
        public string InClear
        {
            get
            {
                return this.m_sInClearText;
            }
            set
            {
                if (this.m_sInClearText != value)
                {
                    this.m_sInClearText = value;
                }
            }
        }

        // sifreli metin için get ve set metodları
        public string Crypted
        {
            get
            {
                return this.m_sCryptedText;
            }
            set
            {
                if (this.m_sCryptedText != value)
                {
                    this.m_sCryptedText = value;
                }
            }
        }
    }
}