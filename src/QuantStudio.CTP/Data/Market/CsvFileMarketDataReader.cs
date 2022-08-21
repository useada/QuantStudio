﻿using CsvHelper;
using CTP;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace QuantStudio.CTP.Data.Market
{
    
    public class CsvFileMarketDataReader : ITransientDependency
    {
        #region private

        IConfiguration _configuration;
        IHostEnvironment _environment;

        #endregion

        #region Ctor / Initialize

        public CsvFileMarketDataReader(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void Initialize()
        {

        }

        #endregion

        public List<CTPMarketData> ReadFiles(string folderName, string fileName)
        {
            // 文件夹
            DirectoryInfo folder = new DirectoryInfo(folderName);
            FileInfo[] files = folder.GetFiles($"*{fileName}*", SearchOption.AllDirectories);
            List<FileInfo> fileNames = new List<FileInfo>();
            fileNames.AddRange(files);

            List<CTPMarketData> cTPMarketDatas = ReadFiles(fileNames);

            return cTPMarketDatas;
        }

        public List<CTPMarketData> ReadFiles(List<FileInfo> fileNames)
        {
            List<CTPMarketData> dictItems = new List<CTPMarketData>();
            foreach(FileInfo fileInfo in fileNames)
            {
                // fileName
                string[] fileNameSplits = fileInfo.Name.Split('_');
                string quoteDate = fileNameSplits[1].Split('.')[0];
                quoteDate = $"{quoteDate.Substring(0, 4)}-{quoteDate.Substring(4, 2)}-{quoteDate.Substring(6, 2)}"; 

                DateOnly date = DateOnly.Parse(quoteDate);
                using (var reader = new StreamReader(fileInfo.FullName))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<CTPMarketData>();
                        csv.Read();
                        csv.ReadHeader();

                        while (csv.Read())
                        {
                            CTPMarketData record = new CTPMarketData(
                                csv.GetField<string>(0), csv.GetField<string>(1), csv.GetField<string>(2), csv.GetField<string>(3), csv.GetField<decimal>(4), csv.GetField<decimal>(5), csv.GetField<decimal>(6), csv.GetField<decimal>(7),
                                 csv.GetField<decimal>(8), csv.GetField<decimal>(9), csv.GetField<decimal>(10), csv.GetField<int>(11), csv.GetField<decimal>(12), csv.GetField<decimal>(13),
                                 csv.GetField<decimal>(14), csv.GetField<decimal>(15), csv.GetField<decimal>(16), csv.GetField<decimal>(17), csv.GetField<decimal>(18), csv.GetField<decimal>(19), Convert.ToDateTime($"{quoteDate} {csv.GetField<string>(20)}"), csv.GetField<int>(21),
                                 csv.GetField<decimal>(22), csv.GetField<int>(23), csv.GetField<decimal>(24), csv.GetField<int>(25), csv.GetField<decimal>(26), csv.GetField<int>(27), csv.GetField<decimal>(28), csv.GetField<int>(29), csv.GetField<decimal>(30), csv.GetField<int>(31), csv.GetField<decimal>(32), csv.GetField<int>(33),
                                 csv.GetField<decimal>(34), csv.GetField<int>(35), csv.GetField<decimal>(36), csv.GetField<int>(37), csv.GetField<decimal>(38), csv.GetField<int>(39), csv.GetField<decimal>(40), csv.GetField<int>(41), csv.GetField<decimal>(42), csv.GetField<string>(43)
                                ); ;

                            dictItems.Add(record);
                        }
                    }
                }
            }

            return dictItems;
        }
    }
}
