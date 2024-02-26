using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcColiseoVirtual.Models;
using MvcColiseoVirtual.Data;

namespace MvcColiseoVirtual.Controllers
{
    public class EscaparateController : Controller
    {
        private readonly MvcTiendaContexto _context;

        public EscaparateController(MvcTiendaContexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {

            var productos = _context.Productos.AsQueryable();

            if (id == null)
            {
                //Selecciona productos del escaparate
                productos = productos.Where(x => x.Escaparate == true);
            }
            else
            {
                //Selecciona productos de la categoría ID
                productos = productos.Where(x => x.CategoriaId == id);

                //Obtiene el nombre de la categoría seleccionada
                ViewBag.DescripcionCategoria = _context.Categorias.Find(id).Descripcion.ToString();
            }

            ViewData["ListaCategorias"] = _context.Categorias.OrderBy(c => c.Descripcion).ToList();

            productos = productos.Include(a => a.Categoria);

            return View(await productos.ToListAsync());
        }

        // GET: Añadir Carrito
        public async Task<IActionResult> AñadirCarrito(int? id)
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

        // POST: Escaparate/AgregarCarrito/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AñadirCarrito(int id)
        {
            //Cargar datos de producto a añadir carrito
            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            //Crear objetos pedido y detalle a agregar
            Pedido pedido = new Pedido();
            Detalle detalle = new Detalle();

            if (User.Identity.IsAuthenticated && await _context.Clientes.AnyAsync(p => p.Email == User.Identity.Name))
            {
                Cliente usuario = await _context.Clientes.Where(p => p.Email == User.Identity.Name).FirstOrDefaultAsync();

                if (HttpContext.Session.GetString("NumPedido") == null && usuario.Id != null)
                {
                    pedido.Fecha = DateTime.Now;
                    pedido.Confirmado = null;
                    pedido.Preparado = null;
                    pedido.Enviado = null;
                    pedido.Cobrado = null;
                    pedido.Devuelto = null;
                    pedido.Anulado = null;
                    pedido.ClienteId = usuario.Id;
                    pedido.EstadoId = 4;

                    if (ModelState.IsValid)
                    {
                        _context.Add(pedido);
                        await _context.SaveChangesAsync();
                    }

                    HttpContext.Session.SetString("NumPedido", pedido.Id.ToString());
                }
            }
            else
            {
                return NotFound();
            }

            //Agregar producto al detalle de un pedido existente
            string strNumeroPedido = HttpContext.Session.GetString("NumPedido");
            detalle.PedidoId = Convert.ToInt32(strNumeroPedido);

            //Verificar si el producto ya está en el carrito del usuario
            var detalleExistente = await _context.Detalles
                .FirstOrDefaultAsync(d => d.PedidoId == Convert.ToInt32(strNumeroPedido) && d.ProductoId == id);

            if (detalleExistente != null)
            {
                //Actualizar cantidad del producto existente
                detalleExistente.Cantidad++;
                _context.Update(detalleExistente);
                await _context.SaveChangesAsync();
            }
            else
            {
                //El valor de id tiene qel ID del producto a agregar
                detalle.ProductoId = id;
                detalle.Cantidad = 1;
                detalle.Precio = producto.Precio;
                detalle.Descuento = 0;

                if (ModelState.IsValid)
                {
                    _context.Add(detalle);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
