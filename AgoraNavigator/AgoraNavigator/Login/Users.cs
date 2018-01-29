﻿using AgoraNavigator.Tasks;
using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AgoraNavigator.Login
{
    class Antena
    {
        public int Id { get; set; }
        public List<User> members { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public int Pin { get; set; }
        public int AntenaId { get; set; }
        public int TotalPoints { get; set; }
        public ObservableCollection<GameTask> openedTasks;
        public ObservableCollection<GameTask> closedTasks;
    }

    class Users
    {
        public static List<Antena> aegeeAntenas;
        private static IFolder rootFolder;
        private static IFolder userDataFolder;
        private static IFile userDataFile;

        public static List<User> users;
        public static User loggedUser;

        public static void InitUsers()
        {
            users = new List<User>();
            User matGrz = new User { Id = 1, Pin = 1234, AntenaId = 1 };
            users.Add(matGrz);
            aegeeAntenas = new List<Antena>();
            Antena cracowAntena = new Antena { Id = 1, members = new List<User>() };
            cracowAntena.members.Add(matGrz);
            aegeeAntenas.Add(cracowAntena);
        }

        public async static Task InitUserData(User user)
        {
            Console.WriteLine("User:InitUserData:user.Id=" + user.Id);
            rootFolder = FileSystem.Current.LocalStorage;
            userDataFolder = await rootFolder.CreateFolderAsync("user_data_" + user.Id, CreationCollisionOption.OpenIfExists);
            try
            {
                userDataFile = await userDataFolder.CreateFileAsync("user_data.txt", CreationCollisionOption.FailIfExists);
                Console.WriteLine("Users:InitUserData:userDataFile=" + userDataFile.Name + " created.");
                loggedUser = user;
                loggedUser.TotalPoints = 0;
                loggedUser.closedTasks = new ObservableCollection<GameTask>();
                loggedUser.openedTasks = new ObservableCollection<GameTask>();
                GameTask.AddTasks();
                String userData = JsonConvert.SerializeObject(loggedUser);
                await userDataFile.WriteAllTextAsync(userData);
            }
            catch (Exception)
            {
                userDataFile = await userDataFolder.CreateFileAsync("user_data.txt", CreationCollisionOption.OpenIfExists);
                Console.WriteLine("Users:InitUserData:userDataFile=" + userDataFile.Name + " already exist.");
                String userData = await userDataFile.ReadAllTextAsync();
                loggedUser = JsonConvert.DeserializeObject<User>(userData);
            }
            Console.WriteLine("Users:InitUserData:loggedUser.TotalPoints=" + loggedUser.TotalPoints);
            Console.WriteLine("Users:InitUserData:loggedUser.openedTasks.Count=" + loggedUser.openedTasks.Count);
            Console.WriteLine("Users:InitUserData:loggedUser.closedTasks.Count=" + loggedUser.closedTasks.Count);
        }

        public async static Task closeTask(GameTask task)
        {
            Console.WriteLine("Users:closeTask:task.id=" + task.id);
            task.completed = true;
            loggedUser.TotalPoints += task.scorePoints;
            loggedUser.openedTasks.Remove(task);
            loggedUser.closedTasks.Add(task);
            String userData = JsonConvert.SerializeObject(loggedUser);
            await userDataFile.WriteAllTextAsync(userData);
            Console.WriteLine("Users:closeTask:loggedUser.TotalPoints=" + loggedUser.TotalPoints);
            Console.WriteLine("Users:closeTask:loggedUser.openedTasks.Count=" + loggedUser.openedTasks.Count);
            Console.WriteLine("Users:closeTask:loggedUser.closedTasks.Count=" + loggedUser.closedTasks.Count);
        }
    }
}
