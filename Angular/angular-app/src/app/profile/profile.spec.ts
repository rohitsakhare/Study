import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { ProfileComponent } from './profile';
import { SupabaseService } from '../supabase.service';
import { routes } from '../app.routes';

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfileComponent],
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
                data: {
                  id: 'u1',
                  full_name: 'Test User',
                  avatar_url: null,
                  bio: 'Hello',
                  membership: 'Social' as const,
                  role: 'user' as const,
                  updated_at: '2026-01-01T00:00:00.000Z',
                },
                error: null,
              }),
            upsertProfile: () =>
              Promise.resolve({
                data: {
                  id: 'u1',
                  full_name: 'Test User',
                  avatar_url: null,
                  bio: 'Hello',
                  membership: 'Social' as const,
                  role: 'user' as const,
                  updated_at: '2026-01-01T00:00:00.000Z',
                },
                error: null,
              }),
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
