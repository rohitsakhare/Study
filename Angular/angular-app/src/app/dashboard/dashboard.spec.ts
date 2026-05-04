import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { DashboardComponent } from './dashboard';
import { SupabaseService } from '../supabase.service';
import { routes } from '../app.routes';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        provideRouter(routes),
        {
          provide: SupabaseService,
          useValue: {
            getUser: () =>
              Promise.resolve({
                data: { user: { id: 'u1', email: 'test@example.com' } },
                error: null,
              }),
            getProfile: () =>
              Promise.resolve({
                data: { id: 'u1', role: 'user' },
                error: null,
              }),
            signOut: () => Promise.resolve({ error: null }),
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
