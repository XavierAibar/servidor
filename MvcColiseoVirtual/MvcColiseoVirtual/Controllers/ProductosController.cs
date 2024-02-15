using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcColiseoVirtual.Data;
using MvcColiseoVirtual.Models;

namespace MvcColiseoVirtual.Controllers
{
    [Authorize(Roles = "Administrador")]

    public class ProductosController : Controller
    {
        private readonly MvcTiendaContexto _context;

        public ProductosController(MvcTiendaContexto context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index(string strCadenaBusqueda, string busquedaActual, int? pageNumber, int? categoriaId, int? categoriaIdActual)
        {

            int pageSize = 2;

            if (pageNumber <= 0)
            {
                pageNumber = 1; 
            }

            if(strCadenaBusqueda != null)
            {
                pageNumber = 1;
                pageSize = 50;
            }

            else
            {
                strCadenaBusqueda = busquedaActual;
            }
            ViewData["BusquedaActual"] = strCadenaBusqueda;

            if(categoriaId != null)
            {
                pageNumber = 1;
                pageSize = 50;
            }
            else
            {
                categoriaId = categoriaIdActual;
            }
            ViewData["categoriaIdActual"] = categoriaId;


            var productos = _context.Productos.AsQueryable();

            //Ordenar productos
            productos = productos.OrderBy(x => x.Descripcion);

            if (!String.IsNullOrEmpty(strCadenaBusqueda))
            {
                productos = productos.Where(s => s.Descripcion.Contains(strCadenaBusqueda));
            }



            if(categoriaId == null)
            {
                ViewData["categoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion");
            }
            else
            {
                ViewData["categoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion", categoriaId);
                productos = productos.Where(x => x.CategoriaId == categoriaId);
            }

            productos = productos.Include(x => x.Categoria);
                        productos.Include(x => x.Detalles);
            return View(await PaginatedList<Producto>.CreateAsync(productos.AsNoTracking(),
            pageNumber ?? 1, pageSize));
            // return View(await _context.Productos.ToListAsync()) :
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Texto,Precio,PrecioCadena,Stock,Escaparate,Imagen,CategoriaId")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion", producto.CategoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Texto,Precio,PrecioCadena,Stock,Escaparate,Imagen,CategoriaId")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Productos == null)
            {
                return Problem("Entity set 'MvcTiendaContexto.Productos'  is null.");
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
          return (_context.Productos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
