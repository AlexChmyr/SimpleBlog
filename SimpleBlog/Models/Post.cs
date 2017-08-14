using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace SimpleBlog.Models
{
    public class Post
    {
        public virtual int Id { get; set; }
        public virtual User User { get; set; }

        public virtual string Title { get; set; }
        public virtual string Slug { get; set; }
        public virtual string Content { get; set; }

        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime? UpdatedAt { get; set; }
        public virtual DateTime? DeletedAt { get; set; }

        public virtual bool IsDeleted { get { return DeletedAt != null; } }

        public virtual IList<Tag> Tags { get; set; }

        public Post()
        {
            Tags = new List<Tag>();
        }
    }

    public class PostMap : ClassMapping<Post>
    {
        public PostMap()
        {
            Table("posts");

            Id(x => x.Id, x => x.Generator(Generators.Identity));

            ManyToOne(p => p.User, p =>
            {
                p.Column("user_id");
                p.NotNullable(true);
            });

            Property(x => x.Title, x => x.NotNullable(true));
            Property(x => x.Slug, x => x.NotNullable(true));
            Property(x => x.Content, x => x.NotNullable(true));

            Property(x => x.CreatedAt, x =>
            {
                x.Column("created_at");
                x.NotNullable(true);
            });

            Property(x => x.UpdatedAt, x => x.Column("updated_at"));
            Property(x => x.DeletedAt, x => x.Column("deleted_at"));

            Bag(t => t.Tags,
                t => {
                    t.Table("post_tags");
                    t.Key(k => k.Column("post_id"));
                },
                t => t.ManyToMany(k => k.Column("tag_id"))
            );

        }
        
       
    }
}