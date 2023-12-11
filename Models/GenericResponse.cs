﻿using System.Net;

namespace ChallengeCFOTech.Models
{
    public class GenericResponse
    {
        public string Message { get; set; }
        public HttpStatusCode HttpCode { get; set; }
        public object Data {  get; set; }   
    }
}
