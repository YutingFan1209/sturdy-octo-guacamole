import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
@Component({
  selector: 'app-root',
  imports: [RouterLink, RouterOutlet],
  template: `<header>
      <a routerLink="/">Employee Operations</a>
      <nav>
        <a routerLink="/requests/new">New request</a><a routerLink="/requests/open">Open request</a>
      </nav>
    </header>
    <main><router-outlet /></main>`,
})
export class App {}
