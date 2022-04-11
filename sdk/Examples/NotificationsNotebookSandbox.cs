using System;
using System.Collections.Generic;
using Finbourne.Notifications.Sdk.Api;
using Finbourne.Notifications.Sdk.Extensions;
using Finbourne.Notifications.Sdk.Model;

namespace Examples
{
    /**
     *  Notifications Test
     *   
     *   This notebook runs through a use case for the notification endpoints. The use case is that, following a data load into LUSID (for example daily feed of Transactions) :
     *   An subscription is created to report the upload event
     *   An email notification is added to the subscription to report the event to a set of interested parties
     *   A webhook notification is added to the subscription to continue processing in a downstream system (this is currently disabled)
     *   The event is manually triggered
     */
    public class NotificationsNotebookSandbox
    {
        private readonly IApiFactory _lusidFactory;
        private readonly SubscriptionsApi _subscriptionsApi;
        private readonly NotificationsApi _notificationsApi;
        private readonly EventsApi _eventsApi;
        
        private readonly string SubscriptionScope= "SubscriptionScope";
        private readonly string SubscriptionCode= "TransactionsLoaded";
        private readonly string DisplayName= "TransactionsLoaded";
        private readonly string EventFilter= "TransactionsLoaded";

        public NotificationsNotebookSandbox()
        {
            var apiSecretsFilename = Environment.GetEnvironmentVariable("FBN_SECRETS_PATH");
            
            _lusidFactory = ApiFactoryBuilder.Build(apiSecretsFilename);
            
            // Setup the apis we'll use in this notebook:
            _subscriptionsApi = _lusidFactory.Api<SubscriptionsApi>();
            _notificationsApi = _lusidFactory.Api<NotificationsApi>();
            _eventsApi = _lusidFactory.Api<EventsApi>();
            
            Console.WriteLine("LUSID Environment Initialised");
            // Console.WriteLine("LUSID SDK Version:");
            
        }


        public void Run()
        {
            /*
             * Create Event Subscriptions
             * 
             * These cells create subscriptions to the events that will be used in the notebook.
             * From these subscriptions notifications can be set up to alert various users or automatically kick off follow on actions.
             */
            CreateEventSubscriptions(
                SubscriptionScope,
                SubscriptionCode,
                DisplayName,
                EventFilter);

            /*
             * Create Email Notifications
             * 
             * This cell sets up email notifications for the events that have been subscribed to above.
             * This means the user will receive an email for each of the events triggered throughout this notebook.
             */
            
            // Create email notifications for the subscription
            CreateEmailNotifications();
            
            // Create manual event for a new issue so that it triggers the email notification
            CreateEvent();
            
            // Remove the subscription
            RemoveSubscription();
        }
        
        // Create subscriptions to our manual events used in this notebook
        private void CreateEventSubscriptions(
            string subscriptionScope,
            string subscriptionCode,
            string displayName,
            string eventFilter)
        {
            var eventSubscription = new CreateSubscription(
                id: new ResourceId(
                    scope: subscriptionScope,
                    code: subscriptionCode),
                displayName: displayName,
                description: "Subscription to a manual event",
                status: "Active",
                matchingPattern: new MatchingPattern(
                    eventType: "Manual",
                    filter: $"Message eq '{eventFilter}'"));
            
            // Create subscription definition
            try
            {
                var createSubscriptionEvent = _subscriptionsApi.CreateSubscription( 
                    createSubscription: eventSubscription);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); // TODO: Do we want to print Data here?
            }
        }
        
        private void CreateEmailNotifications()
        {
            // Create email notifications for the subscription
            var emailAddress = new List<string> {"email"}; // TODO: Populate dynamically
            var createEmailNotification = new CreateEmailNotification(
                description: "Email Event Notification",
                subject: "Email Event Notification",
                plainTextBody: "Event with message and details",
                emailAddressTo: emailAddress
            );

            try
            {
                var createEmailNotificationsResponse = _notificationsApi
                    .CreateEmailNotification(
                        SubscriptionScope,
                        SubscriptionCode,
                        createEmailNotification);

                var notifications = _notificationsApi.ListNotifications(
                    SubscriptionScope,
                    SubscriptionCode);
                
                Console.WriteLine(notifications.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void CreateEvent()
        {
            var students = new Dictionary<string, string>
            {  
                {"EventType", "Manual"},
                {"Id", "abcdefghijk12345"},
                {"Message", "TransactionsLoaded"},
                {"subject", "Email Event Notification"},    
                {"Details", "Test details"},
                {"EventTime", "2021-08-27T17:39:02.9427036+01:00"}
            };

            try
            {
                var createEventResponse = _eventsApi.CreateEvent(
                    students);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private void RemoveSubscription()
        {
           _subscriptionsApi
               .DeleteSubscription(
                   scope: SubscriptionScope,
                   code: SubscriptionCode); 
        }
    }
}
