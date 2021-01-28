using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using System.IO.Ports;

namespace TelegramBotSmart
{
    class Program
    {
       
        private static TelegramBotClient Bot = new TelegramBotClient("1683185634:AAGCoMl0TA_ejqRMnV7PHlbt5YBvZbhyFpw");
        private static bool isAdmin = false;
        private static string text;
        private static long chatid;
        private static string username;
        private static SerialPort port = new SerialPort("COM6", 9600);
        static void Main(string[] args)
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.StartReceiving();
            port.DataReceived += Port_DataReceived;
            Console.ReadLine();
        }

        private async static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (isAdmin)
            {
                await Bot.SendTextMessageAsync(chatid, "intruder");
            }
        }


        #region utilitymessages

        private static void WelcomeMessage()
        {
            if (text.StartsWith("/start"))
            {
                Bot.SendTextMessageAsync(chatid, "Ciao " + username + " Inserisci /pw seguito dalla tua password per continuare");
            }
        }
        private static void PasswordError()
        {
            Bot.SendTextMessageAsync(chatid, "Password non valida");
        }
        private static void AdminMode()
        {
            isAdmin = true;
            Bot.SendTextMessageAsync(chatid, "Sei in modalità admin");
            Bot.SendTextMessageAsync(chatid, "/help per la lista dei comandi");


        }

        #endregion

        #region OnMessageEvent
        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {

            text = e.Message.Text;
            chatid = e.Message.Chat.Id;
            username = e.Message.Chat.Username;

            if (e.Message.Type == MessageType.Text)
            {
                WelcomeMessage();
                if (isAdmin)
                {
                    CheckAdminMessage();
                }
                else
                {
                    CheckMessages();
                }

            }
        }

        #endregion

        #region PWController

        private static int CheckPW()
        {
            if (text.Contains("admin_b"))
            {
                isAdmin = true;
                return 1;
            }
            else
                return -1;
        }

        #endregion

        #region MessageController

        private static void CheckAdminMessage()
        {
            var temp = text.ToLower();
            if (temp.StartsWith("/help"))
            {
                Bot.SendTextMessageAsync(chatid, "/lighton accende la luce\n/lightoff spegne la luce\n/exit Per uscire");
            }
            if (temp.StartsWith("/lighton"))
            {
                Bot.SendTextMessageAsync(chatid, "Accendo la luce");
            }
            if (temp.StartsWith("/lightoff"))
            {
                //todo @Spegni la luce
                Bot.SendTextMessageAsync(chatid, "Spengo la luce");

            }
            if (temp.StartsWith("/exit"))
            {
                Bot.SendTextMessageAsync(chatid, "Ciao " + username + " Inserisci /pw seguito dalla tua password per continuare");
                isAdmin = false;
            }
        }
        private static void CheckMessages()
        {
            if (text.StartsWith("/pw"))
            {
                switch (CheckPW())
                {
                    case 1:
                        AdminMode();
                        break;
                    default:
                        PasswordError();
                        break;
                }
            }
        }

        #endregion

    }

}

