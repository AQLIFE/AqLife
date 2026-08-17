using MyLife.Application.Abstractions.Mapper;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace MyLife.Application.Business.Corpus
{
    [Mapper]
    public partial class CorpusMapper : IViewMapper<CorpusEntity, CorpusDto>, ICreateMapper<CorpusEntity, CreateCorpusCommand>
    {
        public partial CorpusDto ToDto(CorpusEntity source);
        
        public CorpusEntity ToEntity(CreateCorpusCommand command) => new (command.Content);

        public string Convert(DateTime dateTime) => dateTime.ToString(); 
    }
}
