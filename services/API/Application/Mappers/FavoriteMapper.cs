using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.Favorite;
using AqLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Mappers
{
    [Mapper]
    public partial class FavoriteMapper(FavoriteMediaMapper mediaMapper)
        : ICreateMapper<FavoriteItemEntity, CreateFavoriteCommand>,
        IViewMapper<FavoriteItemEntity, FavoriteDto>
    {

        [MapperIgnoreSource(nameof(CreateFavoriteCommand.TagIds))]
        [MapperIgnoreSource(nameof(CreateFavoriteCommand.Media))]
        [MapperIgnoreTarget(nameof(FavoriteItemEntity.Tags))]
        [MapperIgnoreTarget(nameof(FavoriteItemEntity.Media))]
        [MapperIgnoreTarget(nameof(FavoriteItemEntity.UID))]
        [MapperIgnoreTarget(nameof(FavoriteItemEntity.CreatedAt))]
        [MapperIgnoreTarget(nameof(FavoriteItemEntity.UpdatedAt))]
        [MapperIgnoreTarget(nameof(FavoriteItemEntity.IsPublished))]
        [MapperIgnoreTarget(nameof(FavoriteItemEntity.Category))]
        [MapProperty(nameof(CreateFavoriteCommand.CID), nameof(FavoriteItemEntity.CategoryUID))]
        public partial FavoriteItemEntity ToEntity(CreateFavoriteCommand source);

        [MapperIgnoreSource(nameof(FavoriteItemEntity.Category))]
        [MapProperty(nameof(FavoriteItemEntity.Tags), nameof(FavoriteDto.TagIds))]
        [MapProperty(nameof(FavoriteItemEntity.CategoryUID), nameof(FavoriteDto.Cid))]
        [MapProperty(nameof(FavoriteItemEntity.Media), nameof(FavoriteDto.Media))]
        public partial FavoriteDto ToDto(FavoriteItemEntity source);

        public static IEnumerable<Guid> Convert(ICollection<FavoriteTagEntity> source)
            => source.Select(x => x.TagId);

        [UserMapping]
        //[MapperIgnoreSource(nameof(FavoriteMediaEntity.Favorite))]
        [MapperIgnoreSource(nameof(FavoriteMediaEntity.FavoriteId))]
        public IEnumerable<FavoriteMediaDto> Convert(IEnumerable<FavoriteMediaEntity> source)
            => source.Select(mediaMapper.ToDto);
    }

    [Mapper]
    public partial class FavoriteMediaMapper
        : IRelationshipViewMapper<FavoriteMediaEntity, FavoriteMediaDto>,
        IRelationshipUpdateMapper<FavoriteMediaEntity,FavoriteMediaInput>
    {
        //[MapperIgnoreSource(nameof(FavoriteMediaEntity.Favorite))]
        public partial FavoriteMediaDto ToDto(FavoriteMediaEntity source);

        //[MapperIgnoreTarget(nameof(FavoriteMediaEntity.FavoriteId))]
        //[MapperIgnoreTarget(nameof(FavoriteMediaEntity.Favorite))]
        [MapperIgnoreTarget(nameof(FavoriteMediaEntity.Role))]
        public partial FavoriteMediaEntity ToEntity(FavoriteMediaInput source);
    }
}
