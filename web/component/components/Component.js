class {{ComponentClass}} extends HTMLElement {
    static stylesheet;
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });

        if (!{{ComponentClass}}.stylesheet) {
            {{ComponentClass}}.stylesheet = new CSSStyleSheet();
            fetch('./components/{{ComponentClass}}.css').then(res => res.text())
                .then(css => {
                    {{ComponentClass}}.stylesheet.replaceSync(css);
                    this.shadowRoot.adoptedStyleSheets = [{{ComponentClass}}.stylesheet];
                });
        } else {
            this.shadowRoot.adoptedStyleSheets = [{{ComponentClass}}.stylesheet];
        }
    }

    connectedCallback() {
        this.render();
    }

    render() {
        this.shadowRoot.innerHTML = `<div class='{{Component}} ${this.classList}'>${this.innerHTML}</div>`;
    }
}

customElements.define('{{Component}}', {{ComponentClass}});