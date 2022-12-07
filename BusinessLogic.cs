namespace FinalProject;
using System.Data;
using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
class BusinessLogic
{

    static async Task Main(string[] args)
    {
        bool _continue = true;
        User user;
        GuiTier appGUI = new GuiTier();
        DataTier database = new DataTier();

        // start GUI
        user = appGUI.Login();


        if (database.LoginCheck(user))
        {

            while (_continue)
            {
                int option = appGUI.Dashboard(user);
                switch (option)
                {
                    // Addition of a New Package
                    case 1:
                        while (_continue)
                        {
                            Console.WriteLine("Please Input Resident name");
                            string full_name = Console.ReadLine();
                            Console.WriteLine("Please input the apartment unit number");
                            int unit_number = Convert.ToInt16(Console.ReadLine());
                            if (database.PackageCheck(full_name, unit_number))
                            {
                                Console.WriteLine("--------Resident Found; please input the posting service(USPS, Fedex, UPS, DHL)----------");
                                string posting_agency = Console.ReadLine();
                                database.AddToPendingArea(unit_number, full_name, posting_agency);
                                database.AddToPackageHistory(unit_number, full_name, posting_agency);
                                Console.WriteLine("---------The package has been added To Pending Area until pickup--------");
                                DataTable tableAllPending = database.ShowAllPending(user);
                                if (tableAllPending != null)
                                {
                                    appGUI.Display(tableAllPending);
                                }
                                // send email notification
                                string serviceConnectionString = "endpoint=https://moladimejiemailservice.communication.azure.com/;accesskey=79TfLbWn0PO3svW2NCVntC+Dv+shVGJmQBzpvUEIQREQwwlv156DPHxsvojJF6wbhYGaJvxJuypQJrouAy694g==";
                                EmailClient emailClient = new EmailClient(serviceConnectionString);
                                var subject = "A Delivery Has Arrived For You!";
                                var emailContent = new EmailContent(subject);
                                // use Multiline String @ to design html content
                                emailContent.Html = @"
                                            <html>
                                                <body>
                                                    <h1 style=color:blue>Hello VacLife Resident,</h1>
                                                    <h2>A delivery has arrived for you. Please stop by the office to pick it up. Do have a great day!.</h2>
                                                     <p1> Sincerely,
                                                      <br>Your Friends At VacLife Apartments
                                                      <br>806-123-4567
                                                      <br>vaclife@apartmentsunit.com
                                                      </p1>
                                                    </body>
                                            </html>";

                                // mailfrom domain of your email service on Azure
                                var sender = "DoNotReply@f1994192-db24-421e-81b7-ff0ceb330dcb.azurecomm.net";

                                Console.WriteLine("-------------------------Email Queued------------------------");
                                string inputEmail = database.ResidentEmail(unit_number, full_name);
                                var emailRecipients = new EmailRecipients(new List<EmailAddress> {
                                    new EmailAddress(inputEmail) { DisplayName = "Apartment Email" }
                                });

                                var emailMessage = new EmailMessage(sender, emailContent, emailRecipients);

                                try
                                {
                                    SendEmailResult sendEmailResult = emailClient.Send(emailMessage);

                                    string messageId = sendEmailResult.MessageId;
                                    if (!string.IsNullOrEmpty(messageId))
                                    {
                                        Console.WriteLine($"Email sent, MessageId = {messageId}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Failed to send email.");
                                        return;
                                    }

                                    // wait max 2 minutes to check the send status for mail.
                                    var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(2));
                                    do
                                    {
                                        SendStatusResult sendStatus = emailClient.GetSendStatus(messageId);
                                        Console.WriteLine($"Send mail status for MessageId : <{messageId}>, Status: [{sendStatus.Status}]");

                                        if (sendStatus.Status != SendStatus.Queued)
                                        {
                                            break;
                                        }
                                        await Task.Delay(TimeSpan.FromSeconds(10));

                                    } while (!cancellationToken.IsCancellationRequested);

                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        Console.WriteLine($"Looks like we timed out for email");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error in sending email, {ex}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("---------Name Not Found------------");
                                Console.WriteLine("Input Owner Name: ");
                                string packageOwner = Console.ReadLine();
                                Console.WriteLine("Input the posting service");
                                string postServiceAgency = Console.ReadLine();
                                Console.WriteLine("Input delivery date:");
                                string deliveryDate = Console.ReadLine();
                                database.AddToUnknownArea(packageOwner, postServiceAgency, deliveryDate);
                                DataTable showunknownpackage = database.ShowUnknownPackage(user);
                                if (showunknownpackage != null)
                                {
                                    appGUI.DisplayUnknownArea(showunknownpackage);
                                    Console.WriteLine("--------Package added to Unknown area for return to post office---------");
                                }
                            }
                            Console.WriteLine("Please input Yes if you want to add more packages to Pending area and No if you are done (Yes or No):");
                            string answer = Console.ReadLine();
                            if (answer != "Yes")
                            {
                                break;
                            }

                        }
                        break;


                    // Picking up Package
                    case 2:
                        while (_continue)
                        {
                            Console.WriteLine("Input Owner name");
                            string full_name = Console.ReadLine();
                            Console.WriteLine("Input unit number");
                            int unit_number = Convert.ToInt16(Console.ReadLine());
                            if (database.PackageCheck(full_name, unit_number))
                            {
                                Console.WriteLine("---------Pending Package Table After Pickup--------");
                                DataTable tablePending = database.ShowAllPending(user);
                                if (tablePending != null)
                                {
                                    appGUI.Display(tablePending);
                                }
                                database.DeleteFromPending(unit_number, full_name);
                            }
                            else
                            {
                                Console.WriteLine("Incorrect Information, Please try again");
                            }
                            Console.WriteLine("Please input Yes if you want to remove more packages and No if you are done (Yes or No):");
                            string answer = Console.ReadLine();
                            if (answer != "Yes")
                            {
                                break;
                            }
                        }
                        break;
                    // Retrieve package history
                    case 3:
                        Console.WriteLine("----------Package History----------------");

                        DataTable tableRecords = database.ShowHistory(user);
                        if (tableRecords != null)
                        {
                            appGUI.Display(tableRecords);
                        }

                        break;

                    case 4:
                        // Log Out
                        _continue = false;
                        Console.WriteLine("Log Out, Goodbye.");
                        break;

                    // default: wrong input
                    default:
                        Console.WriteLine("Wrong Input");
                        break;
                }

            }
        }
        else
        {
            Console.WriteLine("Login Failed, Goodbye.");
        }
    }
}



