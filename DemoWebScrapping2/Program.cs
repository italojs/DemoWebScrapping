using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DemoWebScrapping2.Models;

namespace DemoWebScrapping2
{
    class Program
    {
        static void Main(string[] args)
        {
            //url base do site ao qual você quer extrair os dados
            var baseUrl = "http://www.freitasleiloesonline.com.br";

            //classe provida pelo HtmlAgilityPack para ser o clinte que vai consumir algum serviço web
            var client = new HtmlWeb();

            //com o metodo Load(url) trazemos o html da pagina para a nossa aplicação
            var pageHome = client.Load(baseUrl + "/homesite/filtro.asp?q=materiais");

            //Aqui nos acessamos os nós do html e selecionamos todos os nós que contem o 
            //id =listaLotesPaginacao && todas as tags ul dentro dessa tag com id=listaLotesPaginacao &&
            //todas as tag li que está dentro da tag ul
            var pagesLenth = pageHome.DocumentNode.SelectNodes("//*[@id='listaLotesPaginacao']/ul/li").Count;
            Console.WriteLine("Paginas encontradas" + pagesLenth);

            //enquanto não acabar as paginas que nós coletamos 
            while (pagesLenth > 0)
            {
                //acesse uma pagina dentro da consulta de materias e baixe o html 
                var currentPage = client.Load(baseUrl + "/homesite/filtro-lotes.asp?q=materiais&txBuscar=&CodSubCategoria=&SubCategoria=&UF=&CodCondicao=&Condicao=&OptValores=0&LblValores=&Regiao=&Cidade=&CodLocal=0&pagina=" + pagesLenth);
                
                //desntro desse html baixado acesse a div com id=listalotes, dentro dessa div acesse as tags ul e as tag li dentro de ul
                //aqui nos estamos obtendo o nó do DOM que contem as informações sobre os produtos
                var materiais = currentPage.DocumentNode.SelectNodes("//div[@id='listaLotes']/ul/li");

                ICollection<Materialmodel> list = new List<Materialmodel>();

                //para cada nó de material que nós pegamos, vamos criar um objeto e guardar no banco de dados
                foreach (var item in materiais)
                {
                    try
                    {
                        //criamos um objeto do tipo Materialmodel novo para cada nó que item temos em materias
                        var material = new Materialmodel();

                        //dentro de item, nós temos parte do DOM da pagina de leilão com as informações de um produto                      
                        //dentro dessa div nós vamos pegar o nó img  da div que tem o id = listafoto, e então pegar o valor do atributo src desse nó
                        material.UrlPicture = item.SelectSingleNode("div[@id='listaFoto']/img").Attributes["src"].Value;
                        //Console.WriteLine(material.UrlPicture);

                        //obtendo o nome do produto do mesmo modo que obtivemos a url da imagem do produto
                        //porem com a diferença que dessa vez nós nao vamos pegar o valor de um atributo
                        //e sim o conteudo dentro dessa tag
                        material.ProductName = item.SelectSingleNode("div[@id='listaDescricao']/h1").InnerHtml;
                        Console.WriteLine(material.ProductName);

                        //aqui nós estamos removendo a tag <h1> dentro da segunda div pois essa tag 'h1' iria nos atrapalhar na proxima etapa
                        item.SelectSingleNode("div[@id='listaDescricao']/h1").Remove();

                        //obtendo a descrição 
                        var description = item.SelectSingleNode("div[@id='listaDescricao']").InnerHtml;
                        material.Description = description.Replace("\t", String.Empty);
                        Console.WriteLine("Descrição: "+material.Description);

                        //obtendo olance inicial
                        //para divs que não tem um id ou algum tipo de identificação, nós usamo o index dela dentro daquele nó(os indexs aqui começam pelo número 1)
                        var initialBid = item.SelectSingleNode("div[@id='listaLances']/div[1]").InnerHtml;
                        material.InitialBid = Regex.Replace(initialBid, @"[^0-9\,\.]", String.Empty); //Regex para pegar apenas o valor do lance inicial dentro da string initialBig
                        //Console.WriteLine("lance inicial: " + material.InitialBid);

                        //obtendo o maior lance
                        var biggestBid = item.SelectSingleNode("div[@id='listaLances']/div[2]").InnerHtml;
                        material.BiggestBid = Regex.Replace(biggestBid, @"[^0-9\,\.]", String.Empty);
                        //Console.WriteLine("Maior lance: " + material.BiggestBid);

                        //obtendo a quantidade de lances.
                        var quantityBids = item.SelectSingleNode("div[@id='listaLances']/div[3]").InnerHtml;
                        material.QuantityBids = Regex.Replace(quantityBids, @"[^0-9]", String.Empty);
                        //Console.WriteLine("Quantidade de lances: " + material.QuantityBids);

                        list.Add(material);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"estourou um exeption no item {materiais.IndexOf(item)} da lista de materias na pagina {pagesLenth}");
                    }
                }

                foreach (var material in list)
                {
                    material.Register();
                }

                pagesLenth--;
            }

            Console.ReadKey();

        }
    }
}
