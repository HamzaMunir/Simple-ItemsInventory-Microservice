using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{

    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;

        public ItemsController(IRepository<Item> itemRepository)
        {

            itemsRepository = itemRepository;
        }

        private static readonly List<ItemDto> items = new(){
            new ItemDto(System.Guid.NewGuid(),"Potion", "", 10, System.DateTimeOffset.UtcNow),
            new ItemDto(System.Guid.NewGuid(),"AntiDot", "", 7, System.DateTimeOffset.UtcNow),
            new ItemDto(System.Guid.NewGuid(),"Sword", "", 40, System.DateTimeOffset.UtcNow)
        };

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = (await itemsRepository.GetAllAsync()).Select(item => item.AsDto());
            return items;
        }

        [HttpGet("items/{id}")]
        public async Task<ActionResult<ItemDto>> GetItemByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto itemDto)
        {
            var item = new Item
            {
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price,
                CreatedData = DateTimeOffset.UtcNow
            };

            await itemsRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetItemByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid Id, UpdateItemDto item)
        {

            var existingItem = await itemsRepository.GetAsync(Id);
            if (existingItem == null)
            {
                return NotFound();
            }
            existingItem.Name = item.Name;
            existingItem.Description = item.Description;
            existingItem.Price = item.Price;
            await itemsRepository.UpdateAsync(existingItem);

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid Id)
        {

            var existingItem = await itemsRepository.GetAsync(Id);
            if (existingItem == null)
            {
                return NotFound();
            }

            await itemsRepository.RemoveAsync(existingItem.Id);

            return NoContent();
        }
    }
}