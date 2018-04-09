using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using RestSharp;
using RestSharp.Authenticators;

namespace BusBoard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var myProgram = new Program();
            myProgram.Start();
        }

        private void Start()
        {
           


            //GetLocation();
            GetPostCode();
          
            // busList = PopulateList(responseList, busList);
            // busList = OrderListByTime(busList);
            //  PrintArrivals(busList);
            Console.ReadLine();
        }

        private void GetPostCode()
        {
            Console.WriteLine("Please enter your postcode:");
            string postcode = Console.ReadLine();
            if (postcode.Count() > 7)
            {
                Console.WriteLine("Sorry that postcode is incorrect");
            }
            else
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient();
                client.BaseUrl = new Uri("https://api.postcodes.io/");
                //TODO: Create a request for Postcode
                var request = new RestRequest("postcodes/" + postcode, Method.GET);
                var postCodeRequest = client.Execute<ResultData>(request);
                string latitude = postCodeRequest.Data.Result.Latitude;
                string longitude = postCodeRequest.Data.Result.Longitude;

                FindNearestStations(latitude, longitude);

            }
        }

        private void FindNearestStations(string latitude, string longitude)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient();
            client.BaseUrl = new Uri("https://api.tfl.gov.uk/StopPoint");
            client.Authenticator = new HttpBasicAuthenticator("17f019cd", "e2d6baee3b1ff00d10071cdd67a96bac");
            var request = new RestRequest("?stopTypes=NaptanOnstreetBusCoachStopPair&useStopPointHierarchy=true&Radius=500&lat="
                + latitude + "&lon=" + longitude, Method.GET);
            Console.WriteLine(longitude + " " + latitude);
            var responseList = client.Execute<ParentBusStop>(request);

            if (responseList.Data.StopPoints.Count >= 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    Console.WriteLine ("[Stop point] " + responseList.Data.StopPoints[i].CommonName + ", [Distance] " + responseList.Data.StopPoints[i].Distance
                        + " Metres, [StopCode] " + responseList.Data.StopPoints[i].LineGroup.ElementAt(0).NaptanIdReference);
                    var busList = new List<BusArrivals>();

                    IRestResponse<List<BusArrivals>> datalist = CallAPI(responseList.Data.StopPoints[i].LineGroup.ElementAt(0).NaptanIdReference);
                    busList = PopulateList(datalist, busList);
                     busList = OrderListByTime(busList);
                      PrintArrivals(busList);

                }
            }
            else
            {
                Console.WriteLine("I'm sorry the information you entered found no results, please make sure you enter a correct postcode");
                GetPostCode();

            }



        }

        private IRestResponse<List<BusArrivals>> CallAPI(string Stopcode)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient();
            client.BaseUrl = new Uri("https://api.tfl.gov.uk/StopPoint/" + Stopcode);
            client.Authenticator = new HttpBasicAuthenticator("17f019cd", "e2d6baee3b1ff00d10071cdd67a96bac");
            var request = new RestRequest("Arrivals", Method.GET);
            var responseList = client.Execute<List<BusArrivals>>(request);
            return responseList;
        }

        private List<BusArrivals> OrderListByTime(List<BusArrivals> busList)
        {
            List<BusArrivals> SortedList = busList.OrderBy(o => o.ExpectedArrival).ToList();
            return SortedList;
        }

        private void PrintArrivals(List<BusArrivals> busList)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Expected Arrival: " + busList.ElementAt(i).ExpectedArrival.ToLocalTime().TimeOfDay + " |||| " + "Vehicle Id: " + busList.ElementAt(i).VehicleId);
            }
        }

        private List<BusArrivals> PopulateList(IRestResponse<List<BusArrivals>> responseList, List<BusArrivals> busList)
        {
            foreach (var item in responseList.Data)
            {
                busList.Add(item);
            }
            return busList;
        }


    }
}
