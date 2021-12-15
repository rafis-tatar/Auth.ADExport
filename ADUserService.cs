using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using Auth.ADExport.Models;

namespace Auth.ADExport
{
    public class ADUserService
    {
        public static List<ADUser> GetAllUsers(string topLevelDoamin, string user, string password)
        {
            var users = new List<ADUser>();

            var usrs = GetUsers(topLevelDoamin, user, password);
            users.AddRange(usrs);

            var domains = GetSubDomains(topLevelDoamin, user, password);

            foreach (Domain domain in domains)
            {
                usrs = GetUsers(domain.Name, user, password);
                users.AddRange(usrs);
            }

            return users;
        }


        /// <summary>
        /// Список доменов
        /// </summary>
        /// <param name="topLevelDoamin"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static DomainCollection GetSubDomains(string topLevelDoamin, string user, string password)
        {
            if (!OperatingSystem.IsWindows()) return null;
            var dr = new DirectoryContext(DirectoryContextType.Domain, topLevelDoamin, user, password);
            var dd = Domain.GetDomain(dr);
            return dd.Children;
        }

        private static string SamAccountNameProperty = "sAMAccountName";
        private static string PrincipalName = "userprincipalname";
        static string CanonicalNameProperty = "CN";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">Domain to query. Should be given in the form ldap://domain.com/ </param>
        /// <returns>A list of users.</returns>
        public static List<ADUser> GetUsers(string domain, string domainUser, string domainPass)
        {
            if (!OperatingSystem.IsWindows()) return null;
            var users = new List<ADUser>();
            using DirectoryEntry searchRoot = new DirectoryEntry($"LDAP://{domain}", domainUser, domainPass);
            using DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot)
            {
                // Set the filter
                Filter = "(&(objectCategory=person)(objectClass=user))"
            };

            // Set the properties to load.
            directorySearcher.PropertiesToLoad.Add(CanonicalNameProperty);
            directorySearcher.PropertiesToLoad.Add(SamAccountNameProperty);
            directorySearcher.PropertiesToLoad.Add(PrincipalName);
            directorySearcher.PropertiesToLoad.Add("department");
            directorySearcher.PropertiesToLoad.Add("distinguishedname");
            directorySearcher.PropertiesToLoad.Add("title");
            directorySearcher.PropertiesToLoad.Add("adspath");
            directorySearcher.PropertiesToLoad.Add("company");
            try
            {
                using var searchResultCollection = directorySearcher.FindAll();
                foreach (SearchResult searchResult in searchResultCollection)
                {
                    string cn = null;
                    string samName = null;
                    string email = null;
                    string department = null;
                    string title = null;
                    string adspath = null;
                    string company = null;


                    if (searchResult.Properties[CanonicalNameProperty].Count > 0)
                        cn = searchResult.Properties[CanonicalNameProperty][0].ToString();

                    if (searchResult.Properties[PrincipalName].Count > 0)
                        email = searchResult.Properties[PrincipalName][0].ToString();

                    if (searchResult.Properties[SamAccountNameProperty].Count > 0)
                        samName = searchResult.Properties[SamAccountNameProperty][0].ToString();

                    if (searchResult.Properties["department"].Count > 0)
                        department = searchResult.Properties["department"][0].ToString();

                    if (searchResult.Properties["title"].Count > 0)
                        title = searchResult.Properties["title"][0].ToString();

                    if (searchResult.Properties["adspath"].Count > 0)
                        adspath = searchResult.Properties["adspath"][0].ToString();

                    if (searchResult.Properties["company"].Count > 0)
                        company = searchResult.Properties["company"][0].ToString();

                    users.Add(new()
                    {
                        UserName = cn, 
                        AccountName = samName, 
                        AccountFullName = email, 
                        UserDept = department,
                        UserPosition = title,
                        AccountPath = adspath,
                        UserCompany = company
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{domain} - {e.Message}");
            }

            Console.WriteLine($"{domain} found {users.Count}");
            return users;
        }
    }
}
