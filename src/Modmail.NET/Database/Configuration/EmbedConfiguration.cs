using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class EmbedConfiguration : IEntityTypeConfiguration<Embed>
{
	public void Configure(EntityTypeBuilder<Embed> builder) {
		builder.Navigation(x => x.Fields)
		       .AutoInclude();

		builder.Navigation(x => x.Author)
		       .AutoInclude();
	}
}