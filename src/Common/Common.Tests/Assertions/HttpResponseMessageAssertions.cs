using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System.Net;

namespace Common.Tests.Assertions;

public class HttpResponseMessageAssertions : ReferenceTypeAssertions<HttpResponseMessage, HttpResponseMessageAssertions>
{
    public HttpResponseMessageAssertions(HttpResponseMessage instance) : base(instance) { }

    protected override string Identifier => "HttpResponseMessage";

    // TODO: Update other tests to use this extension
    public AndConstraint<HttpResponseMessageAssertions> BeStatusCode(HttpStatusCode statusCode, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject)
            .ForCondition(s => s.StatusCode == statusCode)
            .FailWith(GetFailureMessage(statusCode));
        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    private string GetFailureMessage(HttpStatusCode statusCode)
    {
        var body = Subject.Content.ReadAsStringAsync().Result;
        body = EscapeCurlyBraces(body);
        return $"Expected status code '{statusCode}' but got '{Subject.StatusCode}', reason: {Environment.NewLine}{body}";
    }

    private static string EscapeCurlyBraces(string input)
    {
        return input.Replace("{", "{{").Replace("}", "}}");
    }
}

public static class HttpContentExtensions
{
    public static HttpResponseMessageAssertions Should(this HttpResponseMessage instance) => new(instance);
}
