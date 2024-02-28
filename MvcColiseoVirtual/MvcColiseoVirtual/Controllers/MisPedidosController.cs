using System;
using System.Collections.Generic;
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
	[Authorize(Roles = "Usuario")]
	public class MisPedidosController : Controller
	{
		private readonly MvcTiendaContexto _context;

		public MisPedidosController(MvcTiendaContexto context)
		{
			_context = context;
		}


		// GET: MisPedidos
		public async Task<IActionResult> Index(int? pageNumber)
		{

			// Se selecciona el empleado correspondiente al usuario actual
			string emailCliente = User.Identity.Name;

			var cliente = await _context.Clientes.Where(e => e.Email == emailCliente)
			.FirstOrDefaultAsync();


			if (cliente == null)
			{
				return RedirectToAction("Index", "Home");
			}
			// Se seleccionan los avisos del Empleado correspondiente al usuario actual
			var misPedidos = _context.Pedidos
			.Where(a => a.ClienteId == cliente.Id)
			.OrderByDescending(a => a.Fecha)
			.Include(a => a.Cliente)
			.Include(a => a.Detalles)
			.Include(a => a.Estado);
			return View(await misPedidos.ToListAsync());
		}

		// GET: MisPedidos/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var pedido = await _context.Pedidos
				.Include(p => p.Cliente)
				.Include(p => p.Estado)
				.Include(p => p.Detalles)
				.ThenInclude(p => p.Producto)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (pedido == null)
			{
				return NotFound();
			}

			return View(pedido);
		}
	}
}
