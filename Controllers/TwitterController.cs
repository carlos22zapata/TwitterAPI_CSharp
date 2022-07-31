using Api_Tweet1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Tweetinvi.Core.Extensions;
using Microsoft.AspNetCore.Cors;

namespace Api_Tweet1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwitterController : ControllerBase
    {
        /// <summary>
        /// Prueba Branch CAZA
        /// </summary>
        public TwitterController()
        {
        }


        private TwitterClient userTweeterClient()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var APIKey = configuration.GetValue<string>("Authentication:Twitter:APIKey");
            var APISecret = configuration.GetValue<string>("Authentication:Twitter:APISecret");
            var APIToken = configuration.GetValue<string>("Authentication:Twitter:APIToken");
            var APITokenSecret = configuration.GetValue<string>("Authentication:Twitter:APITokenSecret");

            var userClient = new TwitterClient(APIKey, APISecret, APIToken, APITokenSecret);
            return userClient;
        }
               

        /// <summary>
        /// Obtitne la información del usuario que se configura en appsettings. No requiere parametros
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableCors("MyPolicy")]
        [Route("GetUser")]
        public async Task<Object> GetUser()
        {
            TwitterClient userClient = userTweeterClient();
            var user = await userClient.Users.GetAuthenticatedUserAsync();

            var friendIds = await userClient.Users.GetFriendIdsAsync(user.ToString());

            userTw dataTW = new userTw { user = user.ToString(), followers = friendIds.Count() };

            //var friendIds = await userClient.Users.GetFriendsAsync(user.ToString());
            //var friendIdsIterator = userClient.Users.GetFriendIdsIterator(new GetFriendIdsParameters(user.ToString()));

            return dataTW;
        }

        /// <summary>
        /// Obtiene los Tweets que el usuario registrado ha realizado por lista 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableCors("MyPolicy")]
        [Route("GetTweetUser")]
        public async Task<Object> GetTweetUser()
        {
            TwitterClient userClient = userTweeterClient();

            //Obtengo los tweets del usuario logueado
            var homeTimeline = await userClient.Timelines.GetRetweetsOfMeTimelineAsync(); //userClient.Timelines.GetHomeTimelineAsync();
                                                                                          //var Tl = JsonConvert.SerializeObject(homeTimeline.ToArray());

            List<string> tweets = new List<string>();

            foreach (var tw in homeTimeline)
            {
                tweets.Add(tw.ToString());
            }

            var tww = (from w in tweets
                      select new
                      {
                          tweet = w
                      }).ToList();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            return JsonSerializer.Serialize(tww, options); //;            
        }
               

        /// <summary>
        /// Obtiene los tweet de un usuario especifico que se recibe como un parametro string
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [EnableCors("MyPolicy")]
        [Route("GetTweetSpecificUser")]
        public async Task<Object> GetTweetSpecificUser(string user)
        {
            TwitterClient userClient = userTweeterClient();

            try
            {
                // Obtengo los tweets de un usuario en particular
                var userTimeline = await userClient.Timelines.GetUserTimelineAsync(user);
                List<string> tweets = new List<string>();

                foreach (var tw in userTimeline)
                {
                    tweets.Add(tw.ToString());
                }

                return tweets;
            }
            catch
            {
                return "No se encontro el usuario";
            }
        }

        /// <summary>
        /// Obtiene los seguidores de la cuenta logueada con los Token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableCors("MyPolicy")]
        [Route("GetFollowers")]
        public async Task<Object> GetFollowers()
        {
            TwitterClient userClient = userTweeterClient();
            var user = await userClient.Users.GetAuthenticatedUserAsync();

            var Foll = userClient.Users.GetFollowersIterator(new GetFollowersParameters(user)); //.Timelines.GetHomeTimelineIterator();

            List<followers> followers_ = new List<followers>();

            while (!Foll.Completed)
            {
                var p = await Foll.NextPageAsync();

                foreach(var fo in p)
                {
                    string idF = fo.Id.ToString();
                    string nameF = fo.Name.ToString();

                    followers_.Add(new followers
                    {
                        id = idF,
                        name = nameF.ToString()
                    });
                }
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            return JsonSerializer.Serialize(followers_, options);
        }

        /// <summary>
        /// Obtiene los tweets de todas las cuentas posibles sobre un tema en particular que se puede escribir en el filtro que se muestra, por ejemplo
        /// Cocacola o Messi, esto buscará los mensajes que contengan este texto.
        /// </summary>
        /// <param name="find"></param>
        /// <returns></returns>
        [HttpGet]
        [EnableCors("MyPolicy")]
        [Route("GetFilterTweet")]
        public async Task<Object> GetFilterTweet(string find)
        {
            TwitterClient userClient = userTweeterClient();

            var userTimeline = await userClient.Search.SearchTweetsAsync(find);
            List<string> tweets = new List<string>();

            foreach (var tw in userTimeline)
            {
                tweets.Add(tw.ToString());
            }

            return tweets;
        }

        /// <summary>
        /// Esto obtiene los tweets de una cuenta en especifico en relación a un tema que haya dentro de los mensajes separados por un @ entre el usuario y el tema. 
        /// Un ejemplo sería: prueba@carlos22zapata, esto me daría todos los mensajes que hablen del tema prueba que haya publicado carlos22zapata
        /// </summary>
        /// <param name="filterAndUser"></param>
        /// <returns></returns>
        [HttpGet]
        [EnableCors("MyPolicy")]
        [Route("GetFilterTweetSpecificUser")]
        public async Task<Object> GetFilterTweetSpecificUser(string filterAndUser)
        {
            TwitterClient userClient = userTweeterClient();

            int position = filterAndUser.IndexOf("@");
            int Len = filterAndUser.Length - position;

            string user = filterAndUser.Substring(position, Len);
            string search = filterAndUser.Substring(0, position);

            var userTimeline = await userClient.Timelines.GetUserTimelineAsync(user);
            List<string> tweets = new List<string>();

            foreach (var tw in userTimeline)
            {
                int sea = tw.ToString().IndexOf(search);

                if (sea > 0)
                    tweets.Add(tw.ToString());
            }

            return tweets;
        }

        /// <summary>
        /// Se usa para responder un mensaje a un usuario en especifico
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        [EnableCors("MyPolicy")]
        [Route("GetMessages")]
        public async Task<Object> GetMessages(int message)
        {
            TwitterClient userClient = userTweeterClient();
            //var user = await userClient.Users.GetAuthenticatedUserAsync();
            //var userr = await userClient.Users.GetUserAsync(user);

            try
            {
                var prueba = userClient.Messages;

                var messages = await userClient.Messages.GetMessagesAsync();

                var resultado = (from r in messages
                                 select new
                                 {
                                     id = r.Id,
                                     text = r.Text
                                 }).ToList();

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };

                return JsonSerializer.Serialize(resultado, options);

                //return messages.Select(s => s.Text).ToList();
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        /// <summary>
        /// Se usa para responder un mensaje a un usuario en especifico
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPut]
        [EnableCors("MyPolicy")]
        [Route("NewMessage")]
        public async Task<Object> NewMessage(string user, string message)
        {
            TwitterClient userClient = userTweeterClient();
            //var user = await userClient.Users.GetAuthenticatedUserAsync();
            var userr = await userClient.Users.GetUserAsync(user);

            try
            {
                var messageX = await userClient.Messages.PublishMessageAsync(message, userr);
                return "Done...!";
            }
            catch (Exception ex)
            {
            }
            
            return null;
        }

        /// <summary>
        /// Publica un tweet con el contenido de lo que se coloque en el filtro del endpoint 
        /// </summary>
        /// <param name="tweet"></param>
        /// <returns></returns>
        [HttpPut]
        [EnableCors("MyPolicy")]
        public async Task<string> Put(string tweet)
        {
            try
            {
                TwitterClient userClient = userTweeterClient();

                // Para publicar un tweet
                var publishedTweet = await userClient.Tweets.PublishTweetAsync(tweet);

                // Para obtener el tweet que acabo de publicar usando su Id
                var Content = await userClient.Tweets.GetTweetAsync(publishedTweet.Id);

                return "Tweet Id: " + publishedTweet.Id.ToString() + ", publicado! ";
            }
            catch (Exception ex)
            {
                return "Hubo un problema y no pudo publicar el tweet";
            }
        }
    }
  }
