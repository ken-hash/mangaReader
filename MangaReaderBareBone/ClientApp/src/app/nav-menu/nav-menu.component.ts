import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  constructor(private router: Router) { }

  model: any = {};
  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  search() {
    const keyword = this.model.keyword.toLowerCase().trim();
    if (keyword) {
      this.router.navigate(['/Search', keyword]);
      this.model.keyword = '';
    }
  }
}
