using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace P_PassionLecture.Models
{
    public class Book
    {
        public int livre_id { get; set; }
        public string titre { get; set; }
        public int nbPages { get; set; }
        public string extrait { get; set; }
        public string resume { get; set; }
        public DateTime anneeEdition { get; set; }
        public string imageCouverture {  get; set; }
        public DateTime datePublication { get; set; }
        public int editeur_fk { get; set; }
        public int categorie_fk { get; set; }
        public int auteur_fk { get; set; }

        public List<Tag> Tags { get; set; } = new();

    }
}
