using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Common;
using SIGA.Infrastructure.Persistence;

namespace SIGA.Infrastructure.Repositories;

public class Repository<TEntity>(SigaDbContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly SigaDbContext Context = context;
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.FindAsync([id], ct);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.ToListAsync(ct);

    public async Task AddAsync(TEntity entity, CancellationToken ct = default) =>
        await DbSet.AddAsync(entity, ct);

    public void Update(TEntity entity) => DbSet.Update(entity);

    public void Remove(TEntity entity) => DbSet.Remove(entity);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await Context.SaveChangesAsync(ct);
}
