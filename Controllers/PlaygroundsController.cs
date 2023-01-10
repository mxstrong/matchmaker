﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Matchmaker.Models;
using Matchmaker.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Matchmaker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaygroundsController : ControllerBase
    {
        private readonly MatchmakerContext _context;

        public PlaygroundsController(MatchmakerContext context)
        {
            _context = context;
        }

        // GET: api/Playgrounds
        [AllowAnonymous]
        [HttpGet]
        public async Task<List<PlaygroundDto>> GetPlaygrounds()
        {
            var playgrounds = await _context.Playgrounds.ToListAsync();
            
            var playgroundsDtos = playgrounds.Select(p => new PlaygroundDto()
            {
                Id = p.PlaygroundId,
                Name = p.NameOfPlace,
                Size = p.Size
            })
            .ToList();

            return playgroundsDtos;
        }

        // GET: api/Playgrounds/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<PlaygroundDto>> GetPlayground(string id)
        {
            var playground = await _context.Playgrounds.Select(p => new PlaygroundDto()
            {
                Id = p.PlaygroundId,
                Name = p.NameOfPlace,
                Size = p.Size
            }).SingleOrDefaultAsync(p => p.Id == id);

            if (playground == null)
            {
                return NotFound();
            }

            return playground;
        }

        // PUT: api/Playgrounds/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize(Roles = Role.Admin + "," + Role.SuperAdmin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayground(string id, Playground playground)
        {
            if (id != playground.PlaygroundId)
            {
                return BadRequest();
            }

            _context.Entry(playground).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaygroundExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Playgrounds
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize(Roles = Role.Admin + "," + Role.SuperAdmin)]
        [HttpPost]
        public async Task<ActionResult<PlaygroundDto>> PostPlayground(Playground playground)
        {
            playground.PlaygroundId = Guid.NewGuid().ToString();
            _context.Playgrounds.Add(playground);
            await _context.SaveChangesAsync();

            var playgroundDto = new PlaygroundDto()
            {
                Id = playground.PlaygroundId,
                Name = playground.NameOfPlace,
                Size = playground.Size
            };

            return CreatedAtAction("GetPlayground", new { id = playground.PlaygroundId }, playgroundDto);
        }

        // DELETE: api/Playgrounds/5
        [Authorize(Roles = Role.Admin + "," + Role.SuperAdmin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Playground>> DeletePlayground(string id)
        {
            var playground = await _context.Playgrounds.FindAsync(id);
            if (playground == null)
            {
                return NotFound();
            }

            _context.Playgrounds.Remove(playground);
            await _context.SaveChangesAsync();

            return playground;
        }

        private bool PlaygroundExists(string id)
        {
            return _context.Playgrounds.Any(e => e.PlaygroundId == id);
        }
    }
}
