// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using WebsiteCrawler.BenchmarkTests;

var report = BenchmarkRunner.Run<WebPageParserTests>();

Console.ReadKey();
