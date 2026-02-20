using System.Collections.Generic;

namespace ColdStoreManagement.BLL.Data
{
    public class Building
    {
        public int Id { get; set; }
        public string BuildingName { get; set; }

        public ICollection<Chamber> chamber { get; set; }
    }
}
