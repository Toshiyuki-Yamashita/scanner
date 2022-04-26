using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;

namespace scanner.Data.DataSource
{
    internal class BindingEntity<Entity> : BindingList<Entity>
    {
        public BindingEntity() : base()
        {
        }        
    }
}
