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

        byte[] S;

        int x;
        int y;
        string key;

        public HomeController()
        {
            S = new byte[256];
            x = 0;
            y = 0;
            key = "aX45fgcv";
        }

        public void SBox_Olustur(string key)
        {
            for (int i = 0; i < 256; i++)
            {
                S[i] = (byte)i;
            }
            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + S[i] + key[i % key.Length]) % 256;
                Swap(S, i, j);
            }
        }

        public void Swap(byte[] arr, int ind1, int ind2)
        {
            byte temp = arr[ind1];
            arr[ind1] = arr[ind2];
            arr[ind2] = arr[ind1];
        }

        public IActionResult Index()
        {

            RC4ViewModel model = new RC4ViewModel()
            {
                Key = key
            };
            return View(model);
        }


        private byte RastgeleSayiUret()
        {
            x = (x + 1) % 256;
            y = (y + S[x]) % 256;

            Swap(S, x, y);

            return S[(S[x] + S[y]) % 256];
        }

        [HttpPost]
        public IActionResult Index(RC4ViewModel model)
        {
            if (ModelState.IsValid)
            {
                string chiperText = Sifrele(model.PlainText);
                model.ChiperText = chiperText;

                return View(model);
            }
            ModelState.AddModelError("", "Hata");
            return View(model);
        }

        public string Sifrele(string plainText)
        {
            string pT = plainText;
            SBox_Olustur(key);

            byte[] Cipher = new byte[plainText.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                Cipher[i] = (byte)(plainText[i] ^ RastgeleSayiUret());
            }
            string chiperText = "";
            for (int i = 0; i < plainText.Length; i++)
            {
                chiperText += (char)Cipher[i];
            }
            return chiperText;

        }
    }
}