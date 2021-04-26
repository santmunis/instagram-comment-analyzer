using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.NaturalLanguageUnderstanding.v1;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;
using MashupWebApi.Domain.Aggregates.Instagram.Queries;
using MashupWebApi.Extesions;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace MashupWebApi.Domain.Aggregates.Watson.Queries
{
    public class GetTextFeeling 
    {
        public class Contract : IRequest<Result<List<AnalysisResults>>>
        {
            public List<string> texts { get; set; }
        }

        public class Handler : IRequestHandler<Contract, Result<List<AnalysisResults>>>
        {
            private readonly IMediator _mediator;
            private readonly IConfiguration _configuration;

            public Handler(IMediator mediator, IConfiguration configuration)
            {
                _mediator = mediator;
                _configuration = configuration;
            }

            public async Task<Result<List<AnalysisResults>>> Handle(Contract request, CancellationToken cancellationToken)
            {
                if (request.texts.Count == 0)
                    return Result.Fail<List<AnalysisResults>>("Não foi possivél encontrar uma lista de palavras");

                IamAuthenticator authenticator = new IamAuthenticator(
                    apikey: _configuration[$"Watson:apikey"]
                );

                NaturalLanguageUnderstandingService naturalLanguageUnderstanding = new NaturalLanguageUnderstandingService("2021-03-25", authenticator);
                naturalLanguageUnderstanding.SetServiceUrl(_configuration[$"Watson:url"]);

                var features = new Features()
                {
                    Syntax = new SyntaxOptions()
                    {
                        Sentences = true,
                        Tokens = new SyntaxOptionsTokens()
                        {
                            Lemma = true,
                            PartOfSpeech = true
                        }
                    },
                    Categories = new CategoriesOptions()
                    {
                        Limit = 3
                    },
                    Keywords = new KeywordsOptions()
                    {
                        Sentiment = true,
                        Emotion = true,
                        Limit = 2
                    },
                    Sentiment = new SentimentOptions()
                    {
                        Document = true
                    },
                    Emotion = new EmotionOptions()
                    {
                        Document = true
                    }

                };
                var listResult = new List<AnalysisResults>();
                foreach (var text in request.texts)
                {
                    var result = naturalLanguageUnderstanding.Analyze(
                        features: features,
                        returnAnalyzedText: true,
                        language:"pt",
                        text: text
                    );

                    listResult.Add(result.Result);
                }

               

                return Result.Ok(listResult);

            }
        }
    }
}
