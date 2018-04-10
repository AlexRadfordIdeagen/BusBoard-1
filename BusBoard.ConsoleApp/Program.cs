using BusBoard.Api;
using System;
using System.Collections.Generic;

namespace BusBoard.ConsoleApp
{
    public class Program
    {
        private APIController apiCode;
        static void Main(string[] args)
        {
            var myProgram = new Program();
            myProgram.Start();
        }

        public void Start()
        {
            apiCode = new APIController();
            Console.WriteLine("Please enter your postcode:");
            string postcode = Console.ReadLine();
            
            List<string> result = apiCode.GetPostCode(postcode);
            result.ForEach(i => Console.WriteLine(i));
            Console.ReadLine();
        }

        public List<string> FetchBusData(string postcode)
        {
            apiCode = new APIController();

            return apiCode.GetPostCode(postcode);
        }
    }
}
