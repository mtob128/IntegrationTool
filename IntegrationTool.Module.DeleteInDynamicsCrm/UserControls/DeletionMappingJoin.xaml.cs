﻿using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericClasses;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntegrationTool.Module.DeleteInDynamicsCrm.UserControls
{
    /// <summary>
    /// Interaction logic for DeletionMappingJoin.xaml
    /// </summary>
    public partial class DeletionMappingJoin : UserControl
    {
        private IDatastore dataObject;
        private EntityMetadata entityMetadata;

        public DeletionMappingJoin(DeleteInDynamicsCrmConfiguration configuration, EntityMetadata entityMetadata, IDatastore dataObject)
        {
            InitializeComponent();
            this.entityMetadata = entityMetadata;
            this.dataObject = dataObject;
            InitializeMappingControl(configuration.DeleteMapping);
            
        }

        private void InitializeMappingControl(List<DataMappingControl.DataMapping> mapping)
        {
            List<NameDisplayName> sourceList = this.dataObject.Metadata.GetColumnsAsNameDisplayNameList();
            foreach (NameDisplayName item in sourceList)
            {
                deletionMapping.SourceList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            List<NameDisplayName> targetList = Crm2013Wrapper.Crm2013Wrapper.GetAllAttributesOfEntity(entityMetadata);
            foreach (NameDisplayName item in targetList)
            {
                deletionMapping.TargetList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            deletionMapping.Mapping = mapping;

            deletionMapping.RedrawLines();
        }
    }
}
