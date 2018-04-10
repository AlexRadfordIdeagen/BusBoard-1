using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BusBoard.ConsoleApp;
using RestSharp;
using RestSharp.Authenticators;


namespace BusBoard.Api
{
    public class APIController

    {
        public List<string> results = new List<string>();
        public List<string> GetPostCode(string postcode)
        {



            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient();
            client.BaseUrl = new Uri("https://api.postcodes.io/");
            var request = new RestRequest("postcodes/" + postcode, Method.GET);
            var postCodeRequest = client.Execute<ResultData>(request);

            string statusCode = postCodeRequest.StatusCode.ToString();
            if (statusCode == "NotFound")
            {
                results.Add("I'm sorry but that postcode did not work please try again");
                return results;
            }
            else
            {
                string latitude = postCodeRequest.Data.Result.Latitude;
                string longitude = postCodeRequest.Data.Result.Longitude;
                FindNearestStations(latitude, longitude);
                return results;
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
            var responseList = client.Execute<ParentBusStop>(request);

          
            
                for (int i = 0; i < 2; i++)
                {
                    results.Add("The stop point is: " + responseList.Data.StopPoints[i].CommonName + ", it's distance is " + responseList.Data.StopPoints[i].Distance
                        + " Metres away from your postcode, and the stop code is " + responseList.Data.StopPoints[i].LineGroup.ElementAt(0).NaptanIdReference + "\n ");
                    var busList = new List<BusArrivals>();

                    IRestResponse<List<BusArrivals>> datalist = CallAPI(responseList.Data.StopPoints[i].LineGroup.ElementAt(0).NaptanIdReference);
                    busList = PopulateList(datalist, busList);
                    busList = OrderListByTime(busList);
                    PrintArrivals(busList);
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
                string busTime = busList.ElementAt(i).ExpectedArrival.ToLocalTime().ToShortTimeString();
                results.Add("Expected Arrival: " + busTime + " |||| " + "Line Name " + busList.ElementAt(i).LineName + "\n ");
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

