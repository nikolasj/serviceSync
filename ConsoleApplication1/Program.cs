using ConsoleApplication1.Models;
using ConsoleApplication1.ServiceReferenceIIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //DataLayer.SyncCityListWithKudago();
            //DataLayer.SyncCategoryListWithKudago();
            //DataLayer.SyncPlaceListWithKudago();
            //DataLayer.SyncEventListWithKudago();
            //DataLayer.SyncFilmListWithKudago();
            DataLayer.SyncShowListWithKudago();


            //ServiceReferenceIIS.IService1 proxy = new ServiceReferenceIIS.Service1Client();
            //GetDataResponse result = proxy.GetData(new GetDataRequest());
            //Console.WriteLine(result);
            ////proxy.Close();
            //Console.ReadLine();

        }
             
    }
}
