using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MashupWebApi.Extesions;
using MediatR;
using MongoDB.Bson;
using PuppeteerSharp;

namespace MashupWebApi.Domain.Aggregates.Instagram.Queries
{
    public class GetCommentsByPublications
    {
        public class Contract : IRequest<Result<InstagramComments[]>>
        {
            public string UrlPublication { get; set; }
        }

        public class InstagramComments
        {
            public string UserName { get; set; }
            public string Comment { get; set; }
        }


        public class Handler : IRequestHandler<Contract, Result<InstagramComments[]>>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task<Result<InstagramComments[]>> Handle(Contract request, CancellationToken cancellationToken)
            {
                
                var viewPortWidth = 1000;

                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = false,
                    Args = new[]
                    {
                        "--no-sandbox",
                    },

                });

                try
                {
                    var page = await browser.NewPageAsync();

                    await page.SetViewportAsync(new ViewPortOptions
                    {
                        Width = viewPortWidth,
                        Height = 1080,
                        IsMobile = false,
                    });

                    await page.GoToAsync(request.UrlPublication, 
                        new NavigationOptions {
                        Timeout = 0,
                        WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Networkidle0, WaitUntilNavigation.DOMContentLoaded }
                    });

                    await page.WaitForSelectorAsync(".C4VMK", new WaitForSelectorOptions
                    {
                        Timeout = 3000
                    });

                    var divComments = (await page.EvaluateExpressionAsync<InstagramComments[]>(
                        "Array.from(document.querySelectorAll('.C4VMK')).map(x => { return {Comment:x.querySelector('span:nth-child(2)').innerText, UserName : x.querySelector(':nth-child(1)').innerText }});"
                    ));

                    return Result.Ok(divComments);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    await browser.CloseAsync();
                }

            }
        }
    }
}
