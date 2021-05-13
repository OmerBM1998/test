using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace App
{
    class Program
    {
        public enum DataTypes
        {
            FlightId,
            Year,
            Quarter,
            Month,
            DayofMonth,
            DayOfWeek,
            FlightDate,
            UniqueCarrier,
            AirlineID,
            Carrier,
            TailNum,
            FlightNum,
            OriginAirportID,
            OriginAirportSeqID,
            OriginCityMarketID,
            Origin,
            OriginCityName,
            OriginState,
            OriginStateFips,
            OriginStateName,
            OriginWac,
            DestAirportID,
            DestAirportSeqID,
            DestCityMarketID,
            Dest,
            DestCityName,
            DestState,
            DestStateFips,
            DestStateName,
            DestWac,
            CRSDepTime,
            DepTime,
            DepDelay,
            DepDelayMinutes,
            DepDel15,
            DepartureDelayGroups,
            DepTimeBlk,
            TaxiOut,
            WheelsOff,
            WheelsOn,
            TaxiIn,
            CRSArrTime,
            ArrTime,
            ArrDelay,
            ArrDelayMinutes,
            ArrDel15,
            ArrivalDelayGroups,
            ArrTimeBlk,
            Cancelled,
            CancellationCode,
            Diverted,
            CRSElapsedTime,
            ActualElapsedTime,
            AirTime,
            Flights,
            Distance,
            DistanceGroup,
            CarrierDelay,
            WeatherDelay,
            NASDelay,
            SecurityDelay,
            LateAircraftDelay,
            FirstDepTime,
            TotalAddGTime,
            LongestAddGTime,
            DivAirportLandings,
            DivReachedDest,
            DivActualElapsedTime,
            DivArrDelay,
            DivDistance,
            Div1Airport,
            Div1AirportID,
            Div1AirportSeqID,
            Div1WheelsOn,
            Div1TotalGTime,
            Div1LongestGTime,
            Div1WheelsOff,
            Div1TailNum,
            Div2Airport,
            Div2AirportID,
            Div2AirportSeqID,
            Div2WheelsOn,
            Div2TotalGTime,
            Div2LongestGTime,
            Div2WheelsOff,
            Div2TailNum,
            Div3Airport,
            Div3AirportID,
            Div3AirportSeqID,
            Div3WheelsOn,
            Div3TotalGTime,
            Div3LongestGTime,
            Div3WheelsOff,
            Div3TailNum,
            Div4Airport,
            Div4AirportID,
            Div4AirportSeqID,
            Div4WheelsOn,
            Div4TotalGTime,
            Div4LongestGTime,
            Div4WheelsOff,
            Div4TailNum,
            Div5Airport,
            Div5AirportID,
            Div5AirportSeqID,
            Div5WheelsOn,
            Div5TotalGTime,
            Div5LongestGTime,
            Div5WheelsOff,
            Div5TailNum,
            X
        }
        public static List<string[]> DataList;
        //dictionary containing all the departure cities, and all the flights that started from there by line.
        public static Dictionary<string, List<int>> DepartureCityDict;

        /// <summary>
        /// Returns a list of flight data.
        /// </summary>
        public static bool SetDataList()
        {
            Console.WriteLine("please input the csv path without the quotation marks");
            string path = Console.ReadLine();
            if (!File.Exists(path))
            {
                Console.WriteLine("file not found");
                return false;
            }
            using (TextFieldParser parser = new TextFieldParser(path))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    DataList.Add(parser.ReadFields());
                }
            }
            return true;
        }

        public static void SetDepartureCityDict()
        {
            DepartureCityDict = new Dictionary<string, List<int>>();
            for(int i = 0; i < DataList.Count; i++)
            {
                if(DataList[i][(int)DataTypes.Cancelled] == "1")
                {
                    //if flight was canceled, it isn't counted.
                    continue;
                }
                string originCityName = DataList[i][(int)DataTypes.OriginCityName];
                if (DepartureCityDict.ContainsKey(originCityName))
                {
                    DepartureCityDict[originCityName].Add(i);
                }
                else
                {
                    List<int> linePositions = new List<int>();
                    linePositions.Add(i);
                    DepartureCityDict.Add(originCityName, linePositions);
                }
            }
        }

        public static void GetAvarageDelay(string originCity, string destinationCity)
        {
            int numOfFlights = 0;
            float sumOfDepartureDelay = 0, sumOfArrivalDelay = 0;
            if (DepartureCityDict.ContainsKey(originCity))
            {
                foreach(var position in DepartureCityDict[originCity])
                {
                    string[] flightData = DataList[position];
                    if (flightData[(int)DataTypes.DestCityName] == destinationCity)
                    {
                        numOfFlights++;
                        sumOfArrivalDelay += int.Parse(flightData[(int)DataTypes.ArrDelayMinutes]);
                        sumOfDepartureDelay += int.Parse(flightData[(int)DataTypes.DepDelayMinutes]);
                    }
                }
                if (numOfFlights > 0)
                {
                    Console.WriteLine("Based on " + numOfFlights + " flights between " + originCity + " and " + destinationCity + ": \n \t"
                        + "The average departure delay is " + sumOfDepartureDelay / numOfFlights + " minutes \n \t"
                        + "The average arrival delay is " + sumOfArrivalDelay / numOfFlights + " minutes \n \t");
                }
                else
                {
                    Console.WriteLine("No flights from " + originCity + " to " + destinationCity + ". \n");
                }
            }
            else
            {
                Console.WriteLine("No flights from " + originCity + ". \n");
            }
        }

        public static void GetMostPopularAirlineFromCity(string city)
        {
            if (DepartureCityDict.ContainsKey(city))
            {
                Dictionary<string, int> cityAirlineDictionary = new Dictionary<string, int>();
                foreach(int linePos in DepartureCityDict[city])
                {
                    string airport = DataList[linePos][(int)DataTypes.AirTime];
                    if (cityAirlineDictionary.ContainsKey(airport))
                    {
                        cityAirlineDictionary[airport]++;
                    }
                    else
                    {
                        cityAirlineDictionary.Add(airport, 1);
                    }
                }
                var maxValue = cityAirlineDictionary.Values.Max();
                var maxKeyValue = cityAirlineDictionary.FirstOrDefault(x => x.Value == maxValue).Key;
                Console.WriteLine("The most flights taken from " + city + " is " + maxValue + " from airport " + maxKeyValue);
            }
            else
            {
                Console.WriteLine("No flights from " + city + ".");
            }
        }

        public static void GetFarthestAreasFromCity(string city)
        {
            if (DepartureCityDict.ContainsKey(city))
            {
                Dictionary<string, int> cityDistDict = new Dictionary<string, int>();
                foreach (var position in DepartureCityDict[city])
                {
                    string destCity = DataList[position][(int)DataTypes.DestCityName];
                    if (cityDistDict.ContainsKey(destCity))
                    {
                        continue;
                    }
                    cityDistDict.Add(destCity, int.Parse(DataList[position][(int)DataTypes.Distance]));
                }
                var sortedCityDistDict = from entry in cityDistDict orderby entry.Value ascending select entry;
                int numOfCitiesToWrite = cityDistDict.Count < 5 ? cityDistDict.Count : 5;
                for (int i = 0; i < numOfCitiesToWrite; i++)
                {
                    var pair = sortedCityDistDict.ElementAt(i);
                    Console.WriteLine("The number " + i + " most distance taken from " + city + " is " + pair.Value + " to " + pair.Key);
                }
            }
            else
            {
                Console.WriteLine("No flights from " + city + ".");
            }
        }

        public static void GetShortestAvarageArrivalDelayFromOneStop(string originCity, string destinationCity, string stopCity)
        {
            int numOfFlights = 0;
            float minDelay = 0, flightNumber = 0;
            if (DepartureCityDict.ContainsKey(originCity))
            {
                foreach (var position in DepartureCityDict[originCity])
                {
                    string[] flightData = DataList[position];
                    if (flightData[(int)DataTypes.DestCityName] == destinationCity && int.Parse(flightData[(int)DataTypes.Diverted]) == 1 && SearchCityByAirportID(stopCity, flightData[(int)DataTypes.Div1AirportID]))
                    {
                        numOfFlights++;
                        int sumOfArrivalDelay = int.Parse(flightData[(int)DataTypes.ArrDelay]) + int.Parse(flightData[(int)DataTypes.DivArrDelay]);
                        if(minDelay > sumOfArrivalDelay)
                        {
                            minDelay = sumOfArrivalDelay;
                            flightNumber = int.Parse(flightData[(int)DataTypes.FlightId]);
                        }
                    }
                }
                if (numOfFlights > 0)
                {
                    Console.WriteLine("Based on " + numOfFlights + " flights between " + originCity + " and " + destinationCity + 
                        " while stopping in" + stopCity + ", the shortest avarage delay is " + minDelay + " by flight num " + flightNumber);
                }
                else
                {
                    Console.WriteLine("No flights from " + originCity + " to " + destinationCity + "with stops in" + stopCity + ".");
                }
            }
            else
            {
                Console.WriteLine("No flights from " + originCity);
            }
        }

        public static bool SearchCityByAirportID(string city, string airportID)
        {
            foreach(var line in DepartureCityDict[city])
            {
                if(DataList[line][(int)DataTypes.OriginAirportID] == airportID)
                {
                    return true;
                }
            }
            return false;
        }

        static void Main(string[] args)
        {
            DataList = new List<string[]>();
            while (!SetDataList()) ;
            SetDepartureCityDict();
            Console.WriteLine("finished processing");
            while (true)
            {
                Console.WriteLine("what do you want to know?");
                string input = Console.ReadLine();
                string[] recievedInput = input.Split("\"", StringSplitOptions.RemoveEmptyEntries);
                if(recievedInput.Length < 1)
                {
                    Console.WriteLine("you didn't write anything.");
                    continue;
                }
                switch (recievedInput[0].Trim())
                {
                    case "average_delay":
                        if (recievedInput.Length < 4)
                        {
                            Console.WriteLine("you didn't write all the parameters");
                        }
                        else
                        {
                            GetAvarageDelay(recievedInput[1], recievedInput[3]);
                        }
                        break;
                    case "most_flights":
                        if (recievedInput.Length < 2)
                        {
                            Console.WriteLine("you didn't write all the parameters");
                        }
                        else
                        {
                            GetMostPopularAirlineFromCity(recievedInput[1]);
                        }
                        break;

                    case "get_farthest_areas":
                        if (recievedInput.Length < 2)
                        {
                            Console.WriteLine("you didn't write all the parameters");
                        }
                        else
                        {
                            GetFarthestAreasFromCity(recievedInput[1]);
                        }
                        break;
                    case "get_shortest_avarage_arrival _delay_from_one_stop":
                        if (recievedInput.Length < 6)
                        {
                            Console.WriteLine("you didn't write all the parameters");
                        }
                        else
                        {
                            GetShortestAvarageArrivalDelayFromOneStop(recievedInput[1], recievedInput[3], recievedInput[5]);
                        }
                        break;
                    default:
                        Console.WriteLine("nothing came up, please use one of the following commands: \n \t" 
                            + "most_flights, \n \t" + "average_delay, \n \t" + "get_farthest_areas, \n \t" + "get_shortest_avarage_arrival _delay_from_one_stop");
                        break;

                }

            }
        }
    }
}
