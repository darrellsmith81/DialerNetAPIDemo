﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace iSelectManager.Models
{
    public class Campaign
    {
        public string id { get; set; }
        [Display(Name="Campaign")]
        public string DisplayName { get; set; }
        [Display(Name = "Workgroup")]
        public string AcdWorkgroup { get; set; }
        [Display(Name="Contact List")]
        public ContactList ContactList { get; set; }
        [Display(Name="Policy sets")]
        public ICollection<PolicySet> PolicySets { get; set; }

        private ININ.IceLib.Configuration.Dialer.CampaignConfiguration configuration { get; set; }

        public static ICollection<Campaign> find_all()
        {
            List<Campaign> campaigns = new List<Campaign>();

            foreach (var ic_campaign in Application.CampaignConfigurations)
            {
                var campaign = new Campaign(ic_campaign);
                campaigns.Add(campaign);
            }
            return campaigns;
        }

        public static Campaign find(string p_id)
        {
            foreach (var ic_campaign in Application.CampaignConfigurations)
            {
                if (p_id == ic_campaign.ConfigurationId.Id)
                {
                    return new Campaign(ic_campaign);
                }
            }
            return null;
        }

        public static Campaign find_by_name(string p_name)
        {
            foreach (var ic_campaign in Application.CampaignConfigurations)
            {
                if (p_name == ic_campaign.ConfigurationId.DisplayName)
                {
                    return new Campaign(ic_campaign);
                }
            }
            return null;
        }

        public Campaign()
        {
            id = string.Empty;
            DisplayName = string.Empty;
            AcdWorkgroup = string.Empty;
            ContactList = null;
            PolicySets = new List<PolicySet>();
            configuration = null;
        }

        public Campaign(ININ.IceLib.Configuration.Dialer.CampaignConfiguration ic_campaign)
        {
            id = ic_campaign.ConfigurationId.Id;
            DisplayName = ic_campaign.ConfigurationId.DisplayName;
            AcdWorkgroup = ic_campaign.AcdWorkgroup.Value.DisplayName;
            ContactList = ContactList.find(ic_campaign.ContactList.Value.Id);

            PolicySets = new List<PolicySet>();
            foreach (var ic_policyset in ic_campaign.PolicySets.Value)
            {
                PolicySets.Add(PolicySet.find(ic_policyset.Id));
            }
            configuration = ic_campaign;
        }

        public void apply_policies(IEnumerable<string> policy_ids)
        {
            if (policy_ids.Count() == 0)
            {
                return;
            }
            configuration.PrepareForEdit();
            configuration.PolicySets.Value.Clear();
            foreach (var ic_policyset in Application.PolicySetConfigurations)
            {
                if (policy_ids.Contains(ic_policyset.ConfigurationId.Id))
                {
                    configuration.PolicySets.Value.Add(ic_policyset.ConfigurationId);
                }
            }
            configuration.Commit();
        }
    }
}