using System;
using System.Collections.Generic;
using Xunit;

namespace IndependentWork21
{
    // КОД СИСТЕМИ (FACTORY + STRATEGY + SINGLETON + OBSERVER)

    // Factory Method
    public interface IRequest { string Type { get; } }
    public class HttpRequest : IRequest { public string Type => "HTTP"; }
    public class GrpcRequest : IRequest { public string Type => "GRPC"; }

    public abstract class RequestFactory { public abstract IRequest CreateRequest(); }
    public class HttpRequestFactory : RequestFactory { public override IRequest CreateRequest() => new HttpRequest(); }
    public class GrpcRequestFactory : RequestFactory { public override IRequest CreateRequest() => new GrpcRequest(); }

    // Strategy
    public interface IAuthStrategy { string Authenticate(); }
    public class OAuthStrategy : IAuthStrategy { public string Authenticate() => "OAuth Token"; }
    public class BasicAuthStrategy : IAuthStrategy { public string Authenticate() => "Basic Auth"; }

    // Singleton + Observer Context
    public class RequestManager
    {
        private static RequestManager _instance;
        private IAuthStrategy _authStrategy;

        public event Action<string> OnRequestProcessed;

        private RequestManager() { }

        public static RequestManager Instance => _instance ??= new RequestManager();

        public static void ResetForTesting() => _instance = null;

        public void SetAuthStrategy(IAuthStrategy strategy) => _authStrategy = strategy;

        public string ProcessRequest(RequestFactory factory)
        {
            if (_authStrategy == null) 
                throw new InvalidOperationException("Стратегію автентифікації не встановлено!");
            if (factory == null)
                throw new ArgumentNullException(nameof(factory), "Фабрика не може бути порожньою!");

            var request = factory.CreateRequest();
            var authData = _authStrategy.Authenticate();
            
            string result = $"[{request.Type}] Запит виконано з [{authData}]";
            
            OnRequestProcessed?.Invoke(result);
            
            return result;
        }
    }

    public class TestObserver
    {
        public List<string> ReceivedMessages { get; } = new List<string>();
        public void HandleNotification(string message) => ReceivedMessages.Add(message);
    }

    // ІНТЕГРАЦІЙНІ ТЕСТИ

    public class PatternIntegrationTests : IDisposable
    {
        public PatternIntegrationTests()
        {
            RequestManager.ResetForTesting();
        }

        public void Dispose()
        {
            RequestManager.ResetForTesting();
        }

        // Позитивні сценарії

        [Fact]
        public void Positive_SingletonStateIsStable_AcrossCalls()
        {
            var instance1 = RequestManager.Instance;
            var instance2 = RequestManager.Instance;

            Assert.NotNull(instance1);
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void Positive_FullIntegration_HttpWithOAuth_NotifiesObserver()
        {
            var manager = RequestManager.Instance;
            var observer = new TestObserver();
            
            manager.OnRequestProcessed += observer.HandleNotification;
            manager.SetAuthStrategy(new OAuthStrategy());
            var factory = new HttpRequestFactory();

            string result = manager.ProcessRequest(factory);

            Assert.Equal("[HTTP] Запит виконано з [OAuth Token]", result);
            Assert.Single(observer.ReceivedMessages);
            Assert.Equal("[HTTP] Запит виконано з [OAuth Token]", observer.ReceivedMessages[0]);
        }

        [Fact]
        public void Positive_DynamicStrategyChange_RuntimeBehaviorUpdates()
        {
            var manager = RequestManager.Instance;
            var observer = new TestObserver();
            manager.OnRequestProcessed += observer.HandleNotification;
            var factory = new GrpcRequestFactory();

            manager.SetAuthStrategy(new BasicAuthStrategy());
            manager.ProcessRequest(factory);

            manager.SetAuthStrategy(new OAuthStrategy());
            manager.ProcessRequest(factory);

            Assert.Equal(2, observer.ReceivedMessages.Count);
            Assert.Contains("Basic Auth", observer.ReceivedMessages[0]);
            Assert.Contains("OAuth Token", observer.ReceivedMessages[1]);
        }

        // Негативні та граничні сценарії
        [Fact]
        public void Negative_MissingStrategy_ThrowsInvalidOperationException()
        {
            var manager = RequestManager.Instance;
            var factory = new HttpRequestFactory();

            var exception = Assert.Throws<InvalidOperationException>(() => manager.ProcessRequest(factory));
            Assert.Equal("Стратегію автентифікації не встановлено!", exception.Message);
        }

        [Fact]
        public void Edge_ObserverUnsubscribed_DoesNotReceiveNotifications()
        {
            var manager = RequestManager.Instance;
            var observer = new TestObserver();
            manager.SetAuthStrategy(new BasicAuthStrategy());
            
            manager.OnRequestProcessed += observer.HandleNotification;
            manager.OnRequestProcessed -= observer.HandleNotification;
            
            manager.ProcessRequest(new HttpRequestFactory());

            Assert.Empty(observer.ReceivedMessages);
        }
    }
}