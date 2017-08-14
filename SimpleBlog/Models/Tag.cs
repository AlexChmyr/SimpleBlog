using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace SimpleBlog.Models
{
    public class Tag
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Slug { get; set; }

        public virtual IList<Post> Posts { get; set; }

        public Tag()
        {
            Posts = new List<Post>();
        }
    }

    public class TagMap : ClassMapping<Tag>
    {
        public TagMap()
        {
            Table("tags");

            Id(x => x.Id, x => x.Generator(Generators.Identity));

            Property(x => x.Name, x => x.NotNullable(true));
            Property(x => x.Slug, x => x.NotNullable(true));

            Bag(t => t.Posts,
                t => {
                    t.Table("post_tags");
                    t.Key(k => k.Column("tag_id"));
                },
                t => t.ManyToMany(k => k.Column("post_id"))
            );
        }
    }
}