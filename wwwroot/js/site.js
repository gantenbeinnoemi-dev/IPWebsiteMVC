// Seitwaerts scrollbare Bereiche: Mausrad -> seitlich, Pfeile links/rechts (am Ende ausgegraut).
document.querySelectorAll('[data-scroller]').forEach(function (scroller) {
    var area = scroller.querySelector('[data-scroll-area]');
    var links = scroller.querySelector('[data-scroll-links]');
    var rechts = scroller.querySelector('[data-scroll-rechts]');
    if (!area || !links || !rechts) return;

    function aktualisieren() {
        var max = area.scrollWidth - area.clientWidth;
        scroller.toggleAttribute('data-overflow', max > 1);
        links.disabled = area.scrollLeft <= 1;
        rechts.disabled = area.scrollLeft >= max - 1;
    }

    function scrollen(richtung) {
        area.scrollBy({ left: richtung * area.clientWidth * 0.8, behavior: 'smooth' });
    }

    links.addEventListener('click', function () { scrollen(-1); });
    rechts.addEventListener('click', function () { scrollen(1); });
    area.addEventListener('scroll', aktualisieren, { passive: true });

    // Mausrad ueber dem Bereich scrollt seitwaerts. Am Ende der Reihe scrollt die Seite normal weiter.
    area.addEventListener('wheel', function (e) {
        if (Math.abs(e.deltaX) > Math.abs(e.deltaY)) return;
        var max = area.scrollWidth - area.clientWidth;
        if (max <= 1) return;
        var nachRechts = e.deltaY > 0;
        if (nachRechts && area.scrollLeft >= max - 1) return;
        if (!nachRechts && area.scrollLeft <= 0) return;
        e.preventDefault();
        area.scrollLeft += e.deltaMode === 1 ? e.deltaY * 32 : e.deltaY;
    }, { passive: false });

    new ResizeObserver(aktualisieren).observe(area);
    window.addEventListener('load', aktualisieren);
    aktualisieren();
});

// Sicherheitsabfrage vor dem Loeschen: <button data-confirm="..."> oder <form data-confirm="...">
document.addEventListener('submit', function (e) {
    var text = (e.submitter && e.submitter.dataset.confirm) || e.target.dataset.confirm;
    if (text && !window.confirm(text)) e.preventDefault();
});

// Vorschau des gewaehlten Cover-Bildes im Bearbeiten-Formular
document.querySelectorAll('input[type=file][data-vorschau]').forEach(function (input) {
    input.addEventListener('change', function () {
        var img = document.getElementById(input.dataset.vorschau);
        var leer = document.getElementById('cover-leer');
        var datei = input.files && input.files[0];
        if (!img || !datei) return;
        img.src = URL.createObjectURL(datei);
        img.hidden = false;
        if (leer) leer.hidden = true;
    });
});

// Eintrag aus dem Entwurf einer neuen Kategorie entfernen (die versteckte BuchIds-Eingabe verschwindet mit der Zeile)
document.addEventListener('click', function (e) {
    var knopf = e.target.closest('[data-entfernen]');
    if (!knopf) return;
    var zeile = knopf.closest('[data-eintrag]');
    if (zeile) zeile.remove();
});

// Suchfeld der Buecherauswahl: filtert nach Titel, Reihe und Autor; leere Autorgruppen werden ausgeblendet
document.querySelectorAll('[data-auswahl-suche]').forEach(function (feld) {
    var zeilen = document.querySelectorAll('[data-auswahl-zeile]');
    var gruppen = document.querySelectorAll('[data-auswahl-gruppe]');
    var leer = document.querySelector('[data-auswahl-leer]');
    feld.addEventListener('input', function () {
        var suche = feld.value.trim().toLowerCase();
        zeilen.forEach(function (z) { z.hidden = suche !== '' && z.dataset.suche.indexOf(suche) === -1; });
        var sichtbar = 0;
        gruppen.forEach(function (g) {
            var hat = g.querySelector('[data-auswahl-zeile]:not([hidden])') !== null;
            g.hidden = !hat;
            if (hat) sichtbar++;
        });
        if (leer) leer.hidden = sichtbar > 0 || gruppen.length === 0;
    });
});
