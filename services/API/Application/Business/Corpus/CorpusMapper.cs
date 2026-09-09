using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Business.Corpus
{
    [Mapper]
    public partial class CorpusMapper : IViewMapper<CorpusEntity, CorpusDto>, ICreateMapper<CorpusEntity, CreateCorpusCommand>
    {
        public partial CorpusDto ToDto(CorpusEntity source);

        public CorpusEntity ToEntity(CreateCorpusCommand command) => new(command.Content);

        public string Convert(DateTime dateTime) => dateTime.ToString();
    }
}
