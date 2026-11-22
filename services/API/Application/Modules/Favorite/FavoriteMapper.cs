using Core.Search;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.Favorite;
using Riok.Mapperly.Abstractions;
using System.Collections;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Favorite
{
    [Mapper]
    public partial class FavoriteMapper(FavoriteMediaMapper mediaMapper)
        : ICreateMapper<FavoriteItemEntity, CreateFavoriteCommand>,
        IViewMapper<FavoriteItemEntity, FavoriteDto>,
        ISearchMapper<FavoriteQuery, PageResult<FavoriteDto>, FavoriteSearchCriteria>
    {
        [MapperIgnoreSource(nameof(FavoriteQuery.Page))]
        [MapperIgnoreSource(nameof(FavoriteQuery.PageSize))]
        [MapProperty(nameof(FavoriteQuery.CName), nameof(FavoriteSearchCriteria.Keyword))]
        public partial FavoriteSearchCriteria ToCriteria(FavoriteQuery query);

        //[MapProperty(nameof(CreateFavoriteCommand.Media), nameof(FavoriteItemEntity.Media),Use = nameof(MediaBuilder))]
        //[MapProperty(nameof(CreateFavoriteCommand.TagIds), nameof(FavoriteItemEntity.Tags),Use =nameof(TagBuilder))]
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
        [MapProperty(nameof(CreateFavoriteCommand.Title), nameof(FavoriteItemEntity.Title))]
        [MapProperty(nameof(CreateFavoriteCommand.Description), nameof(FavoriteItemEntity.Description))]
        [MapProperty(nameof(CreateFavoriteCommand.SortOrder), nameof(FavoriteItemEntity.SortOrder))]
        [MapProperty(nameof(CreateFavoriteCommand.SourceUrl), nameof(FavoriteItemEntity.SourceUrl))]
        private partial FavoriteItemEntity MapToEntity(CreateFavoriteCommand source);

        public FavoriteItemEntity ToEntity(CreateFavoriteCommand source)
        {
            FavoriteItemEntity entity = MapToEntity(source);
            entity.Media = MediaBuilder(source.Media, entity.UID).ToList();
            entity.Tags = TagBuilder(source.TagIds, entity.UID).ToList();
            return entity;
        }

        public static IEnumerable<FavoriteMediaEntity> MediaBuilder(IEnumerable<Guid>? mediaIds,Guid favoriteId)
        => mediaIds?.Distinct().Select(e => new FavoriteMediaEntity(favoriteId: favoriteId, fileId: e)) ?? [];

        public static IEnumerable<FavoriteTagEntity> TagBuilder(IEnumerable<Guid>? tagIds, Guid favoriteId) 
        => tagIds?.Distinct().Select(e => new FavoriteTagEntity(favoriteId:favoriteId , tagId: e)) ?? [];


        [MapperIgnoreSource(nameof(FavoriteItemEntity.Category))]
        [MapProperty(nameof(FavoriteItemEntity.Tags), nameof(FavoriteDto.TagIds),Use = nameof(Convert))]
        [MapProperty(nameof(FavoriteItemEntity.CategoryUID), nameof(FavoriteDto.Cid))]
        [MapProperty(nameof(FavoriteItemEntity.Media), nameof(FavoriteDto.Media),Use = nameof(MapToFavoriteMediaDto))]
        public partial FavoriteDto ToDto(FavoriteItemEntity source);

        public static IEnumerable<Guid> Convert(ICollection<FavoriteTagEntity> source)
            => source.Select(x => x.TagId);

        [UserMapping]
        //[MapperIgnoreSource(nameof(FavoriteMediaEntity.Favorite))]
        [MapperIgnoreSource(nameof(FavoriteMediaEntity.FavoriteId))]
        public IEnumerable<FavoriteMediaDto> MapToFavoriteMediaDto(IEnumerable<FavoriteMediaEntity> source)
            => source.Select(mediaMapper.ToDto);
    }

    [Mapper]
    public partial class FavoriteMediaMapper
        : IRelationshipViewMapper<FavoriteMediaEntity, FavoriteMediaDto>,
        IRelationshipUpdateMapper<FavoriteMediaEntity, FavoriteMediaInput>
    {
        //[MapperIgnoreSource(nameof(FavoriteMediaEntity.Favorite))]
        public partial FavoriteMediaDto ToDto(FavoriteMediaEntity source);

        //[MapperIgnoreTarget(nameof(FavoriteMediaEntity.FavoriteId))]
        //[MapperIgnoreTarget(nameof(FavoriteMediaEntity.Favorite))]
        [MapperIgnoreTarget(nameof(FavoriteMediaEntity.Role))]
        public partial FavoriteMediaEntity ToEntity(FavoriteMediaInput source);
    }
}
