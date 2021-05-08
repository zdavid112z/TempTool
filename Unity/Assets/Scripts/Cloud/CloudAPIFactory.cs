using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CloudAPI
{
    public class CloudAPIFactory
    {
        private const string kLocalhostBaseURI = "http://localhost:8080/api";
        private const string kProdBaseURI = "http://temptool.org/api";

        public static ICloudAPI CreateDefaultAPI()
        {
            string isProd = Environment.GetEnvironmentVariable("TEMPTOOL_PROD");
            string baseURI = kLocalhostBaseURI;
            if (isProd == "1")
                baseURI = kProdBaseURI;
            return new CloudAPI(baseURI);
        }
    }
}
