
(function () {
    'use strict';

    // Marca con "tiene-scroll" las tarjetas cuya tabla es más ancha que el espacio disponible.
    // El CSS fija entonces la columna de acciones a la derecha para que Eliminar nunca se pierda.
    function marcarDesborde() {
        Array.prototype.forEach.call(document.querySelectorAll('.table-card'), function (c) {
            c.classList.toggle('tiene-scroll', c.scrollWidth > c.clientWidth + 1);
        });
    }

    function iniciarDesborde() {
        marcarDesborde();
        window.addEventListener('resize', marcarDesborde);
        if (window.ResizeObserver) {
            var ro = new ResizeObserver(marcarDesborde);
            Array.prototype.forEach.call(
                document.querySelectorAll('.table-card, .table-card .data-table'),
                function (el) { ro.observe(el); }
            );
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', iniciarDesborde);
    } else {
        iniciarDesborde();
    }

    window.crearPaginador = function (opciones) {
        var porPagina = opciones.porPagina || 10;
        var cont = document.querySelector(opciones.controles);
        var pagina = 1;

        // Si la vista no renderizó el contenedor (por ejemplo, no hay datos), no hace nada.
        if (!cont) return { irA: function () { }, refrescar: function () { } };

        function listas() {
            return opciones.grupos.map(function (sel) {
                return Array.prototype.slice.call(document.querySelectorAll(sel));
            });
        }

        function numerosDePagina(total) {
            var out = [];
            for (var p = 1; p <= total; p++) {
                if (p === 1 || p === total || Math.abs(p - pagina) <= 1) out.push(p);
                else if (out[out.length - 1] !== '…') out.push('…');
            }
            return out;
        }

        function render() {
            var grupos = listas();
            var base = grupos[0];

            // Posiciones que pasan el filtro
            var validos = [];
            base.forEach(function (el, i) {
                if (!el.classList.contains('fuera-filtro')) validos.push(i);
            });

            var totalPaginas = Math.max(1, Math.ceil(validos.length / porPagina));
            if (pagina > totalPaginas) pagina = totalPaginas;
            if (pagina < 1) pagina = 1;

            var desde = (pagina - 1) * porPagina;
            var hasta = desde + porPagina;

            var enPagina = {};
            validos.forEach(function (idx, pos) {
                enPagina[idx] = pos >= desde && pos < hasta;
            });

            // Se aplica por posición a todos los grupos (tabla y cards)
            grupos.forEach(function (lista) {
                lista.forEach(function (el, i) {
                    el.classList.toggle('fuera-pagina', !enPagina[i]);
                });
            });

            if (validos.length === 0) {
                cont.innerHTML = '';
                return;
            }

            var html = '<span class="paginacion-info">Mostrando ' + (desde + 1) + '–' +
                Math.min(hasta, validos.length) + ' de ' + validos.length + '</span>';

            if (totalPaginas > 1) {
                html += '<div class="paginacion-btns">';
                html += '<button type="button" data-p="' + (pagina - 1) + '"' +
                    (pagina === 1 ? ' disabled' : '') + ' aria-label="Anterior">‹</button>';

                numerosDePagina(totalPaginas).forEach(function (p) {
                    if (p === '…') {
                        html += '<span class="puntos">…</span>';
                    } else {
                        html += '<button type="button" data-p="' + p + '"' +
                            (p === pagina ? ' class="activa" aria-current="page"' : '') + '>' + p + '</button>';
                    }
                });

                html += '<button type="button" data-p="' + (pagina + 1) + '"' +
                    (pagina === totalPaginas ? ' disabled' : '') + ' aria-label="Siguiente">›</button>';
                html += '</div>';
            }

            cont.innerHTML = html;
        }

        cont.addEventListener('click', function (e) {
            var btn = e.target.closest('button[data-p]');
            if (!btn || btn.disabled) return;
            pagina = parseInt(btn.getAttribute('data-p'), 10);
            render();

            var tarjeta = cont.closest('.table-card');
            if (tarjeta && tarjeta.scrollIntoView) {
                tarjeta.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });

        render();

        return {
            irA: function (p) { pagina = p || 1; render(); },
            refrescar: function () { render(); }
        };
    };
})();